using SQLite;
using HirveeProjekti.Models;

namespace HirveeProjekti.Services
{
    public class DatabaseInitializer
    {
        private readonly SQLiteConnection _db;

        public DatabaseInitializer(SQLiteConnection db)
        {
            _db = db;
        }

        public void Initialize()
        {
            try
            {
                // Create tables
                _db.CreateTable<Area>();
                _db.CreateTable<Cottage>();
                _db.CreateTable<Customer>();
                _db.CreateTable<Booking>();
                _db.CreateTable<Service>();
                _db.CreateTable<Invoice>();

                System.Diagnostics.Debug.WriteLine("All tables created successfully");

                // Check if data already exists
                var areaCount = _db.Table<Area>().Count();
                System.Diagnostics.Debug.WriteLine($"Area count: {areaCount}");
                
                if (areaCount == 0)
                {
                    System.Diagnostics.Debug.WriteLine("No data found, inserting sample data...");
                    InsertSampleData();
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("Database already contains data");
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Database initialization error: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }

        private void InsertSampleData()
        {
            try
            {
                // Insert Areas
                var areas = new List<Area>
                {
                    new Area { Nimi = "Lappi" },
                    new Area { Nimi = "Karelia" },
                    new Area { Nimi = "Turku" },
                    new Area { Nimi = "Helsinki" },
                    new Area { Nimi = "Vaasa" }
                };
                _db.InsertAll(areas);
                System.Diagnostics.Debug.WriteLine($"Inserted {areas.Count} areas");

                // Insert Customers
                var customers = new List<Customer>
                {
                    new Customer { Postinro = "00100", Etunimi = "Jari", Sukunimi = "Virtanen", Lahiosoite = "Männikötie 5", Email = "jari.virtanen@email.com", Puhelinnro = "0501234567" },
                    new Customer { Postinro = "00200", Etunimi = "Kaisa", Sukunimi = "Mäkinen", Lahiosoite = "Puusepäntie 10", Email = "kaisa.makinen@email.com", Puhelinnro = "0502345678" },
                    new Customer { Postinro = "20100", Etunimi = "Mikko", Sukunimi = "Korhonen", Lahiosoite = "Kalevantie 15", Email = "mikko.korhonen@email.com", Puhelinnro = "0503456789" },
                    new Customer { Postinro = "96200", Etunimi = "Liisa", Sukunimi = "Nieminen", Lahiosoite = "Ounasvaarantie 20", Email = "liisa.nieminen@email.com", Puhelinnro = "0504567890" },
                    new Customer { Postinro = "80100", Etunimi = "Pekka", Sukunimi = "Rantanen", Lahiosoite = "Siikatie 25", Email = "pekka.rantanen@email.com", Puhelinnro = "0505678901" },
                    new Customer { Postinro = "65100", Etunimi = "Anna", Sukunimi = "Seppälä", Lahiosoite = "Hovioikeudenk. 30", Email = "anna.seppala@email.com", Puhelinnro = "0506789012" }
                };
                _db.InsertAll(customers);
                System.Diagnostics.Debug.WriteLine($"Inserted {customers.Count} customers");

                // Insert Cottages
                var cottages = new List<Cottage>
                {
                    new Cottage { AlueId = 1, Postinro = "96200", Mokkinimi = "Lapin Talvimökki", Katuosoite = "Tunturintie 1", Hinta = 150.00, Kuvaus = "Lämmin mökki poronhiihdon keskellä", Henkilomaara = 6, Varustelu = "Sauna, poreallas, kaakao" },
                    new Cottage { AlueId = 1, Postinro = "96400", Mokkinimi = "Koillis-Lapin Lumipesä", Katuosoite = "Lumihiukkastie 5", Hinta = 120.00, Kuvaus = "Pieni perhemökki lumipeitteisen maiseman äärellä", Henkilomaara = 4, Varustelu = "Sauna, TV" },
                    new Cottage { AlueId = 2, Postinro = "80100", Mokkinimi = "Karjalan Kaunis Koti", Katuosoite = "Pielisniementie 10", Hinta = 100.00, Kuvaus = "Perinteinen mökki Pielisen rannalla", Henkilomaara = 5, Varustelu = "Sauna, grilli, uimarantaa" },
                    new Cottage { AlueId = 2, Postinro = "80200", Mokkinimi = "Joen Ranta Mökki", Katuosoite = "Joensuuntie 15", Hinta = 110.00, Kuvaus = "Moderni mökki kalastajien paratiisissa", Henkilomaara = 4, Varustelu = "Sauna, keittiö, pesula" },
                    new Cottage { AlueId = 3, Postinro = "20100", Mokkinimi = "Turun Saaristomökki", Katuosoite = "Saaristotie 20", Hinta = 95.00, Kuvaus = "Kesämökki saaressa", Henkilomaara = 3, Varustelu = "Grilli, terassi" },
                    new Cottage { AlueId = 4, Postinro = "00100", Mokkinimi = "Helsingin Lähellä", Katuosoite = "Metrotie 25", Hinta = 130.00, Kuvaus = "Rauhallinen mökki kaupungin lähellä", Henkilomaara = 5, Varustelu = "Sauna, pysäköinti, patio" },
                    new Cottage { AlueId = 5, Postinro = "65100", Mokkinimi = "Pohjanmaan Perhemökki", Katuosoite = "Rantatakatie 30", Hinta = 105.00, Kuvaus = "Ystävällinen mökki perheiden suosikki", Henkilomaara = 6, Varustelu = "Sauna, leikkikenttä, grilli" }
                };
                _db.InsertAll(cottages);
                System.Diagnostics.Debug.WriteLine($"Inserted {cottages.Count} cottages");

                // Insert Services
                var services = new List<Service>
                {
                    new Service { AlueId = 1, Nimi = "Koirasledit", Kuvaus = "Opastetut koirasleliajeluut Lapin lumilla", Hinta = 75.00, Alv = 24.0 },
                    new Service { AlueId = 1, Nimi = "Poronhoito", Kuvaus = "Poron ohjaus ja koulutus", Hinta = 85.00, Alv = 24.0 },
                    new Service { AlueId = 2, Nimi = "Kalastus", Kuvaus = "Ohjattu kalastus Pielisessä", Hinta = 60.00, Alv = 24.0 },
                    new Service { AlueId = 2, Nimi = "Sauna & Spa", Kuvaus = "Perinteinen sauna ja hieronta", Hinta = 50.00, Alv = 24.0 },
                    new Service { AlueId = 3, Nimi = "Saaristoristeilyt", Kuvaus = "Veneilyt Turun saaristossa", Hinta = 65.00, Alv = 24.0 },
                    new Service { AlueId = 4, Nimi = "Kaupunkikierros", Kuvaus = "Helsingin nähtävyyksien kierros", Hinta = 45.00, Alv = 24.0 },
                    new Service { AlueId = 5, Nimi = "Pyöräilyreitit", Kuvaus = "Ohjatut pyöräilyreitit Pohjanmaalla", Hinta = 40.00, Alv = 24.0 }
                };
                _db.InsertAll(services);
                System.Diagnostics.Debug.WriteLine($"Inserted {services.Count} services");

                // Insert Bookings
                var today = DateTime.Now;
                var bookings = new List<Booking>
                {
                    new Booking { AsiakasId = 1, MokkiId = 1, VarattuPvm = today, VahvistusPvm = today, VarattuAlkuPvm = today.AddDays(10), VarattuLoppuPvm = today.AddDays(17) },
                    new Booking { AsiakasId = 2, MokkiId = 3, VarattuPvm = today, VahvistusPvm = today, VarattuAlkuPvm = today.AddDays(15), VarattuLoppuPvm = today.AddDays(22) },
                    new Booking { AsiakasId = 3, MokkiId = 5, VarattuPvm = today, VahvistusPvm = today, VarattuAlkuPvm = today.AddDays(5), VarattuLoppuPvm = today.AddDays(12) }
                };
                _db.InsertAll(bookings);
                System.Diagnostics.Debug.WriteLine($"Inserted {bookings.Count} bookings");

                // Insert Invoices
                var invoices = new List<Invoice>
                {
                    new Invoice { VarausId = 1, Summa = 1500.00, Alv = 360.00, Maksettu = false },
                    new Invoice { VarausId = 2, Summa = 1200.00, Alv = 288.00, Maksettu = true },
                    new Invoice { VarausId = 3, Summa = 900.00, Alv = 216.00, Maksettu = false }
                };
                _db.InsertAll(invoices);
                System.Diagnostics.Debug.WriteLine($"Inserted {invoices.Count} invoices");

                System.Diagnostics.Debug.WriteLine("Sample data inserted successfully");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error inserting sample data: {ex.Message}");
                System.Diagnostics.Debug.WriteLine($"Stack trace: {ex.StackTrace}");
            }
        }
    }
}
