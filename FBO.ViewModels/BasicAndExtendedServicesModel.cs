using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FBO.ViewModels
{
    public class BasicAndExtendedServicesModel
    {
        public FBOManagement_UpdateBasicServices_Result BasicServices = new FBOManagement_UpdateBasicServices_Result();
        public FBOManagement_UpdateExtendedServices_Result ExtendedService = new FBOManagement_UpdateExtendedServices_Result();
    }
    public class FBOManagement_UpdateBasicServices_Result
    {

        public string companyID { get; set; }
        public bool checkboxCatering { get; set; }
        public bool checkboxHotel { get; set; }
        public bool checkboxCourtesy { get; set; }
        public bool checkboxWeather { get; set; }
        public bool checkboxRepairs { get; set; }
        public bool checkboxRentalCars { get; set; }

    }
    public class FBOManagement_UpdateExtendedServices_Result
    {
        public string companyID { get; set; }
        public bool checkboxPilotLounge { get; set; }
        public bool checkboxBroadBand { get; set; }
        public bool checkboxParking { get; set; }
        public bool checkboxRestaurant { get; set; }
        public bool checkboxInternetAccess { get; set; }
        public bool checkboxRestrooms { get; set; }
        public bool checkboxShowers { get; set; }
        public bool checkboxCrewCars { get; set; }
        public bool checkboxPublicTelephone { get; set; }
        public bool checkboxAircraftDetailing { get; set; }
        public bool checkboxAircraftParts { get; set; }
        public bool checkboxFlyingClub { get; set; }
        public bool checkboxAircraftMods { get; set; }
        public bool checkboxAircraftPainting { get; set; }
        public bool checkboxAircraftInterior { get; set; }
        public bool checkboxAirMaintenance { get; set; }
        public bool checkboxPowMaintenance { get; set; }
        public bool checkboxAvionicsService { get; set; }
        public bool checkboxPassengerTerminal { get; set; }
        public bool checkboxAircraftRental { get; set; }
        public bool checkboxCharters { get; set; }
        public bool checkboxOxygen { get; set; }
        public bool checkboxHangars { get; set; }
        public bool checkboxTieDowns { get; set; }
        public bool checkboxFlightInstruction { get; set; }
        public bool checkboxLavService { get; set; }
        public bool checkboxQuickTurn { get; set; }
        public bool checkboxDeIcing { get; set; }
        public bool checkboxSnoozeRoom { get; set; }
        public bool checkboxTelevision { get; set; }
        public bool checkboxConference { get; set; }
        public bool checkboxVending { get; set; }
        public bool checkboxFlightPlanning { get; set; }

        public bool checkboxBusinessCenter { get; set; }
        public bool checkboxFitnessCenter { get; set; }
    }
}
