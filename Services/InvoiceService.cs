
using HirveeProjekti.Models;
namespace HirveeProjekti.Services
{
    public class InvoiceService
    {
        private readonly List<Invoice> _invoices = new();

        private readonly BookingService _bookingService;
        private readonly ServiceOfBookingsService _serviceOfBookingsService;
        private readonly List<Service> _services; // palvelut (hinta + alv)

        private int _nextInvoiceId = 1;

        public InvoiceService(
            BookingService bookingService,
            ServiceOfBookingsService serviceOfBookingsService,
            List<Service> services)
        {
            _bookingService = bookingService;
            _serviceOfBookingsService = serviceOfBookingsService;
            _services = services;
        }

        // Laskun muodostaminen
        public Invoice CreateInvoice(int varausId)
        {
            var booking = _bookingService
                .GetAllBookings()
                .FirstOrDefault(b => b.VarausId == varausId);

            if (booking == null)
                throw new Exception("Varausta ei l�ytynyt.");

            // Y�t
            int nights = (booking.VarattuLoppuPvm - booking.VarattuAlkuPvm).Days;
            if (nights <= 0)
                nights = 1;

            // M�kin hinta
            double mokkiSumma = nights * booking.Mokki.Hinta;

            // Palveluiden summa
            var palvelut = _serviceOfBookingsService.GetByVarausId(varausId);

            double palveluSumma = 0;
            double palveluAlv = 0;

            foreach (var item in palvelut)
            {
                var palvelu = _services.FirstOrDefault(p =>
                    p.PalveluId == item.PalveluId);

                if (palvelu == null)
                    continue;

                var riviSumma = item.Lkm * palvelu.Hinta;
                palveluSumma += riviSumma;
                palveluAlv += riviSumma * (palvelu.Alv / 100.0);
            }

            // Oletetaan, ett� m�kin hinta sis�lt�� saman ALV:n (24%)
            double mokkiAlv = mokkiSumma * 0.24;

            var invoice = new Invoice
            {
                LaskuId = _nextInvoiceId++,
                VarausId = varausId,
                Summa = mokkiSumma + palveluSumma,
                Alv = mokkiAlv + palveluAlv,
                Maksettu = false
            };

            _invoices.Add(invoice);
            return invoice;
        }

        // Laskujen haku
        public List<Invoice> GetAllInvoices()
        {
            return _invoices;
        }

        public Invoice? GetInvoiceById(int laskuId)
        {
            return _invoices.FirstOrDefault(i => i.LaskuId == laskuId);
        }

        public List<Invoice> GetUnpaidInvoices()
        {
            return _invoices.Where(i => !i.Maksettu).ToList();
        }

        // Maksun seuranta
        public void MarkAsPaid(int laskuId)
        {
            var invoice = GetInvoiceById(laskuId);
            if (invoice != null)
            {
                invoice.Maksettu = true;
            }
        }

        //  Paperi / Email 
        public void PrintInvoice(int laskuId)
        {
            var invoice = GetInvoiceById(laskuId);
            if (invoice == null)
                throw new Exception("Laskua ei l�ytynyt.");

            // T�ss� vaiheessa vain placeholder-logiikka
            Console.WriteLine($"Tulostetaan lasku #{invoice.LaskuId}");
        }

        public void SendInvoiceByEmail(int laskuId)
        {
            var invoice = GetInvoiceById(laskuId);
            if (invoice == null)
                throw new Exception("Laskua ei l�ytynyt.");

            // Placeholder
            Console.WriteLine($"L�hetet��n s�hk�postilasku #{invoice.LaskuId}");
        }
    }
}
