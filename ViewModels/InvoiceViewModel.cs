using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using HirveeProjekti.Models;
using HirveeProjekti.Services;
using Microsoft.Maui.Controls;
using SQLite;

namespace HirveeProjekti.ViewModels
{
    public class InvoiceViewModel : BindableObject
    {
        private readonly SQLiteConnection _db;
        private readonly BookingService _bookingService;
        private readonly ServiceService _serviceService;
        private readonly CottageService _cottageService;
        private readonly CustomerService _customerService;

        public ObservableCollection<Invoice> Invoices { get; set; } = new();
        public ObservableCollection<Booking> AvailableBookings { get; set; } = new();

        private Booking? _selectedBookingForInvoice;
        public Booking? SelectedBookingForInvoice
        {
            get => _selectedBookingForInvoice;
            set
            {
                _selectedBookingForInvoice = value;
                OnPropertyChanged();
            }
        }

        private string _invoiceActionMessage = string.Empty;
        public string InvoiceActionMessage
        {
            get => _invoiceActionMessage;
            set
            {
                _invoiceActionMessage = value;
                OnPropertyChanged();
                OnPropertyChanged(nameof(HasInvoiceActionMessage));
            }
        }

        public bool HasInvoiceActionMessage => !string.IsNullOrWhiteSpace(InvoiceActionMessage);

        private Invoice? _selectedInvoice;
        public Invoice? SelectedInvoice
        {
            get => _selectedInvoice;
            set
            {
                _selectedInvoice = value;
                OnPropertyChanged();
                if (value != null)
                {
                    LoadSelectedInvoiceDetails();
                }
                else
                {
                    SelectedInvoiceDetails = string.Empty;
                }
            }
        }

        private string _selectedInvoiceDetails = string.Empty;
        public string SelectedInvoiceDetails
        {
            get => _selectedInvoiceDetails;
            set
            {
                _selectedInvoiceDetails = value;
                OnPropertyChanged();
            }
        }

        public ICommand SelectInvoiceCommand { get; }
        public ICommand TogglePaymentStatusCommand { get; }
        public ICommand SendReminderCommand { get; }
        public ICommand CreateInvoiceFromBookingCommand { get; }

        public InvoiceViewModel()
        {
            var dbPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "vn.db");

            _db = new SQLiteConnection(dbPath);

            // Initialize database first
            var initializer = new DatabaseInitializer(_db);
            initializer.Initialize();

            _bookingService = new BookingService();
            _serviceService = new ServiceService();
            _cottageService = new CottageService();
            _customerService = new CustomerService();

            SelectInvoiceCommand = new Command<Invoice?>(SelectInvoice);
            TogglePaymentStatusCommand = new Command<int>(ToggleInvoicePaymentStatus);
            SendReminderCommand = new Command<int>(SendReminder);
            CreateInvoiceFromBookingCommand = new Command(CreateInvoiceFromBooking);

            LoadInvoices();
            LoadAvailableBookings();
        }

        private void LoadInvoices()
        {
            try
            {
                Invoices.Clear();
                System.Diagnostics.Debug.WriteLine("Starting to load invoices...");
                
                var invoices = _db.Table<Invoice>().ToList();
                System.Diagnostics.Debug.WriteLine($"Query returned {invoices.Count} invoices from database");
                
                foreach (var invoice in invoices)
                {
                    Invoices.Add(invoice);
                    System.Diagnostics.Debug.WriteLine($"Added invoice: ID={invoice.LaskuId}, VarausId={invoice.VarausId}, Summa={invoice.Summa}");
                }

                System.Diagnostics.Debug.WriteLine($"Loaded {Invoices.Count} invoices total");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading invoices: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }

        private void SelectInvoice(Invoice? invoice)
        {
            SelectedInvoice = invoice;
        }

        private void LoadAvailableBookings()
        {
            try
            {
                AvailableBookings.Clear();

                var invoicedBookingIds = _db.Table<Invoice>()
                    .Select(i => i.VarausId)
                    .ToHashSet();

                var bookings = _bookingService.GetAllBookings()
                    .Where(b => !invoicedBookingIds.Contains(b.VarausId))
                    .OrderBy(b => b.VarattuAlkuPvm)
                    .ToList();

                foreach (var booking in bookings)
                {
                    AvailableBookings.Add(booking);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading available bookings: {ex.Message}");
            }
        }

        private void CreateInvoiceFromBooking()
        {
            try
            {
                if (SelectedBookingForInvoice == null)
                {
                    InvoiceActionMessage = "Valitse varaus ennen laskun luontia.";
                    return;
                }

                bool invoiceExists = _db.Table<Invoice>()
                    .Any(i => i.VarausId == SelectedBookingForInvoice.VarausId);

                if (invoiceExists)
                {
                    InvoiceActionMessage = "Valitulle varaukselle on jo olemassa lasku.";
                    LoadAvailableBookings();
                    return;
                }

                int dayCount = (SelectedBookingForInvoice.VarattuLoppuPvm - SelectedBookingForInvoice.VarattuAlkuPvm).Days;
                if (dayCount <= 0)
                {
                    dayCount = 1;
                }

                var cottage = SelectedBookingForInvoice.Mokki
                    ?? _cottageService.GetAllCottages().FirstOrDefault(c => c.MokkiId == SelectedBookingForInvoice.MokkiId);

                if (cottage == null)
                {
                    InvoiceActionMessage = "Varauksen mökin tietoja ei löytynyt.";
                    return;
                }

                var total = cottage.Hinta * dayCount;
                var vat = total * 0.24;

                var invoice = new Invoice
                {
                    VarausId = SelectedBookingForInvoice.VarausId,
                    Summa = total,
                    Alv = vat,
                    Maksettu = false
                };

                _db.Insert(invoice);
                InvoiceActionMessage = $"Lasku luotu varauksesta #{SelectedBookingForInvoice.VarausId}: {total:F2} EUR.";

                SelectedBookingForInvoice = null;
                LoadInvoices();
                LoadAvailableBookings();
            }
            catch (Exception ex)
            {
                InvoiceActionMessage = $"Laskun luonti epäonnistui: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"Error creating invoice: {ex.Message}");
            }
        }

        private void LoadSelectedInvoiceDetails()
        {
            try
            {
                if (SelectedInvoice == null)
                {
                    SelectedInvoiceDetails = string.Empty;
                    return;
                }

                var details = new System.Text.StringBuilder();
                details.AppendLine($"🧾 Lasku ID: {SelectedInvoice.LaskuId}");
                details.AppendLine($"📋 Varaus ID: {SelectedInvoice.VarausId}");
                details.AppendLine();

                // Get booking details
                var booking = _bookingService.GetAllBookings()
                    .FirstOrDefault(b => b.VarausId == SelectedInvoice.VarausId);

                if (booking != null)
                {
                    details.AppendLine("📅 Varauksen tiedot:");
                    details.AppendLine($"  • Asiakas: {booking.Asiakas?.FullName ?? "Tuntematon"}");
                    details.AppendLine($"  • Mökki: {booking.Mokki?.Mokkinimi ?? "Tuntematon"}");
                    details.AppendLine($"  • Alkupäivä: {booking.VarattuAlkuPvm:dd.MM.yyyy}");
                    details.AppendLine($"  • Loppupäivä: {booking.VarattuLoppuPvm:dd.MM.yyyy}");
                    
                    int nights = (booking.VarattuLoppuPvm - booking.VarattuAlkuPvm).Days;
                    if (nights <= 0) nights = 1;
                    details.AppendLine($"  • Yöt: {nights}");
                    details.AppendLine($"  • Mökin hinta/yö: {booking.Mokki?.Hinta:F2} €");
                }

                details.AppendLine();
                details.AppendLine("💰 Laskun tiedot:");
                details.AppendLine($"  • Yhteensä: {SelectedInvoice.Summa:F2} €");
                details.AppendLine($"  • ALV: {SelectedInvoice.Alv:F2} €");
                details.AppendLine($"  • Maksettu: {(SelectedInvoice.Maksettu ? "Kyllä" : "Ei")}");

                SelectedInvoiceDetails = details.ToString();
            }
            catch (Exception ex)
            {
                SelectedInvoiceDetails = $"Virhe tietojen lataamisessa: {ex.Message}";
                System.Diagnostics.Debug.WriteLine($"Error loading details: {ex.Message}");
            }
        }

        private void ToggleInvoicePaymentStatus(int laskuId)
        {
            try
            {
                var invoice = Invoices.FirstOrDefault(i => i.LaskuId == laskuId);
                if (invoice != null)
                {
                    invoice.Maksettu = !invoice.Maksettu;
                    _db.Update(invoice);

                    var statusText = invoice.Maksettu ? "Maksettu" : "Maksamatta";
                    InvoiceActionMessage = $"Laskun #{invoice.LaskuId} tila päivitetty: {statusText}.";

                    int? selectedInvoiceId = SelectedInvoice?.LaskuId;
                    LoadInvoices();
                    if (selectedInvoiceId.HasValue)
                    {
                        SelectedInvoice = Invoices.FirstOrDefault(i => i.LaskuId == selectedInvoiceId.Value);
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error toggling payment status: {ex.Message}");
            }
        }

        private void SendReminder(int laskuId)
        {
            try
            {
                var invoice = Invoices.FirstOrDefault(i => i.LaskuId == laskuId);
                if (invoice != null && !invoice.Maksettu)
                {
                    System.Diagnostics.Debug.WriteLine($"Reminder sent for invoice {laskuId}");
                    // Placeholder for sending reminder (email, SMS, etc.)
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error sending reminder: {ex.Message}");
            }
        }

        public void RefreshInvoices()
        {
            LoadInvoices();
            LoadAvailableBookings();
            System.Diagnostics.Debug.WriteLine($"Invoices refreshed - Total: {Invoices.Count}");
        }
    }
}