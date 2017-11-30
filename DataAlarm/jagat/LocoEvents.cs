using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WabtecOneAPI
{


    public class Loco //NO ---Locoid list 
    {
        public string trainId { get; set; }
        public string locomotiveId { get; set; }
        //public string locomotiveIds { get; set; }
        public string onboardSoftwareVersion { get; set; }
        public string departureLocation { get; set; }
        public string initializationTime { get; set; }
        public string departureTime { get; set; }
        public int delay { get; set; }
        public string arrivalLocation { get; set; }
        public string arrivalTime { get; set; }
        public int activeTime { get; set; }
        public string locostate { get; set; }
        public string locostatesummery { get; set; }
        public int enforcementCount { get; set; }
        public int InitializationCount { get; set; }
        public string associationTime { get; set; }
        public string disassociation { get; set; }
        //public int PrimeryLocoId { get; set; }




    }
    public class LocoHealth //NO ---Locoid list 
    {
        public string trainId { get; set; }
        public string locomotiveId { get; set; }
        public string onboardSoftwareVersion { get; set; }
        public string departureLocation { get; set; }
        public string initializationTime { get; set; }
        public string departureTime { get; set; }
        public int delay { get; set; }
        public string arrivalLocation { get; set; }
        public string arrivalTime { get; set; }
        public int activeTime { get; set; }
        public string locostate { get; set; }
        public string locostatesummery { get; set; }
        public int enforcementCount { get; set; }
        public int InitializationCount { get; set; }
        public string associationTime { get; set; }
        public string disassociation { get; set; }
        public int PrimeryLocoId { get; set; }

        public string incorrectconsist { get; set; }
        public string triplexmismatch { get; set; }
        public string eabcommloss { get; set; }
        public string throttlewiringissue { get; set; }
        public string hornwiringissue { get; set; }
        public string onboardecr { get; set; }
        public string other { get; set; }
        public string issuetype { get; set; }
        public string issuetime { get; set; }


        public string group { get; set; }
        public string region { get; set; }
        public string ptcstate { get; set; }
        public string engineer { get; set; }


    }
    public class LocoInit
    {
        public string trainId { get; set; }
        public string locomotiveId { get; set; }
        public string initializationdate { get; set; }
        public string initsuccessrate { get; set; }
        public string runningaverage { get; set; }

        public string group { get; set; }
        public string region { get; set; }
        public string ptcstate { get; set; }
        public string engineer { get; set; }

    }
    public class LocoEnforcement
    {
        public string trainId { get; set; }
        public string locomotiveId { get; set; }
        public string initializationdate { get; set; }
        public string enforcementrate { get; set; }
        public string enforcementavg { get; set; }

        public string group { get; set; }
        public string region { get; set; }
        public string ptcstate { get; set; }
        public string engineer { get; set; }

        public string enforcementdate { get; set; }

        public string enforcementtype { get; set; }
        public string enforcementtarget { get; set; }
        public string latitude { get; set; }
        public string longitude { get; set; }
        public string maplatitude { get; set; }
        public string maplongitude { get; set; }



    }
    public class LocoEnrouteFail
    {
        public string trainId { get; set; }
        public string locomotiveId { get; set; }
        public string initializationdate { get; set; }
        public string enroutefailurerate { get; set; }
        public string runningaverage { get; set; }
        public string group { get; set; }
        public string region { get; set; }
        public string ptcstate { get; set; }
        public string engineer { get; set; }
    }

    public class LocoDetails
    {
        public string trainId { get; set; }
        public string locomotiveId { get; set; }
        public string symbol { get; set; }
        public string route { get; set; }
        public string lastcommtime { get; set; }
        public string lastdepttest { get; set; }
        public string ptcstate { get; set; }
        public string ptcs_state { get; set; }
        public string speed { get; set; }
        public string h_mp { get; set; }
        public string r_mp { get; set; }
        public string sub { get; set; }
        public string direction { get; set; }
        public string sw_version { get; set; }
        public string emprr { get; set; }
        public string o_rr { get; set; }
        public string activefaultcount { get; set; }
        public string bosreadiness { get; set; }
        public string locoreadiness { get; set; }
        public string group { get; set; }
        public string region { get; set; }
        public string engineer { get; set; }

    }

    public class LocoEnforcement_Real
    {

        public string locomotiveId { get; set; }

        public string trainId { get; set; }
        public string Msglogtime { get; set; }
        public int messagetype { get; set; }
        public string scacgroup { get; set; }
        public string region { get; set; }
        public string enforcementtype { get; set; }
        public string onboardSoftwareVersion { get; set; }
        public string targetType { get; set; }
        public string enforcementtarget { get; set; }
        public string warninglat { get; set; }
        public string warninglon { get; set; }
        public string enforcementlat { get; set; }
        public string enforcementlog { get; set; }
        public int trainspeed { get; set; }
        public string currentlat { get; set; }
        public string currentlon { get; set; }

    }
    public class Locolatlng
    {
        public string latitude { get; set; }
        public string longitude { get; set; }
    }

    public class latlngDetails
    {
        //public string LocoID { get; set; }
        //public string locomotiveId { get; set; }
        // public string locoNum { get; set; 


        public string locoID { get; set; }
        public string messageTime { get; set; }
        public int messageTimeInt { get; set; }
        public string railroadSCAC { get; set; }
        public string headEndTrackName { get; set; }
        public string headEndPTCSubdiv { get; set; }
        public string headEndMilepost { get; set; }
        public string headEndCurrentPositionLat { get; set; }
        public string headEndCurrentPositionLon { get; set; }
        public string directionOfTravel { get; set; }
        public string locomotiveState { get; set; }
    }

    public class dayinlife
    {
        public string locomotiveId { get; set; }
    }

    public class subdiv
    {
        public string headEndPTCSubdiv { get; set; }
    }

    public class LocoEnforcement_Dash
    {

        public string trainID { get; set; }
        public string locoID { get; set; }
        public string warningEnforcement { get; set; }
        public string warning_enforcement_date_time { get; set; }
        public string warnEnforcementType { get; set; }
        public string OnboardSoftwareVersion { get; set; }
        public string targetType { get; set; }
        public string targetDescription { get; set; }
        public int targetStartMilepost { get; set; }
        public string targetStartTrackName { get; set; }
        public int targetEndMilepost { get; set; }
        public string targetEndTrackName { get; set; }
        public int targetSpeed { get; set; }
        public object warning_date_time { get; set; }
        public int warningStartMilepost { get; set; }
        public string warningStartTrackName { get; set; }
        public int warningDistance { get; set; }
        public string warningDirectionOfTravel { get; set; }
        public int warningTrainSpeed { get; set; }
        public string enforcement_date_time { get; set; }
        public int enforcementStartMilepost { get; set; }
        public string enforcementStartTrackName { get; set; }
        public int enforcementBrakingDistance { get; set; }
        public string enforcementDirectionOfTravel { get; set; }
        public int enforcementTrainSpeed { get; set; }
        public string emergencyBraking_date_time { get; set; }
        public int emergencyStartMilepost { get; set; }
        public string emergencyStartTrackName { get; set; }
        public int emergencyBrakingDistance { get; set; }
        public string emergencyDirectionOfTravel { get; set; }
        public int emergencyTrainSpeed { get; set; }
        public string current_date_time { get; set; }
        public int currentMilepost { get; set; }
        public string currentTrackName { get; set; }
        public string currentDirectionOfTravel { get; set; }
        public int currentTrainSpeed { get; set; }

    }
    public class stringlineclass
    {
        public string locoID { get; set; }
        public string messageTime { get; set; }
        public int messageTimeInt { get; set; }
        public string railroadSCAC { get; set; }
        public string headEndTrackName { get; set; }
        public int headEndPTCSubdiv { get; set; }
        public int headEndMilepost { get; set; }
        public double headEndCurrentPositionLat { get; set; }
        public double headEndCurrentPositionLon { get; set; }
        public string directionOfTravel { get; set; }
        public string locomotiveState { get; set; }
    }

    public class enfreal
    {
        public string locomotiveId { get; set; }
        public string enforcementtype { get; set; }
        public string enforcementtarget { get; set; }
        
    }

    public class enfreallatlng
    {
        public string enforcementtype { get; set; }
        public string Enforcement_Lat { get; set; }
        public string Enforcement_Lon { get; set; }

    }




}
