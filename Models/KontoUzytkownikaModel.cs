﻿namespace CarRental.Models
{
    public class KontoUzytkownikaModel
    {
        public int IdKontoUzytkownika { get; set; }
        public string Login { get; set; }

        public string Haslo { get; set; }
        
        public bool CzyPracownik { get; set; }

    }

}
