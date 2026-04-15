using System.Collections.Generic;
using HirveeProjekti.Models;

namespace HirveeProjekti.Models
{
    // palvelu taulu 
    public class Service
    {
        public int PalveluId { get; set; }   
        public int AlueId { get; set; }      
        public string Nimi { get; set; }    
        public string Kuvaus { get; set; } 
        public double Hinta { get; set; }    
        public double Lkm { get; set; }      
    }
}