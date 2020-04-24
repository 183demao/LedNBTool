using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace NbIotCmd.Entity
{
    [Table("TNL_TunnelLight")]
    public class TNL_TunnelLight  
    {
        [Key]
        public long TunnelLight_ID { get; set; }
        public long TunnelSection_ID { get; set; }
        public long TunnelGateway_ID { get; set; }
        public long Tunnel_ID { get; set; }
        public string LightPhysicalAddress_TX { get; set; }
        public int LightLocationNumber_NR { get; set; }
        public string LightType_TX { get; set; }
        public string PowerType_TX { get; set; }
        public float VoltageHighValue_NR { get; set; }
        public float VoltageLowValue_NR { get; set; }
        public float CurrentHighValue_NR { get; set; }
        public float CurrentLowValue_NR { get; set; }
        public float Longitude { get; set; }
        public float Latitude { get; set; }
        public int LightUsage_NR { get; set; }
        public string Mileage_TX { get; set; }
        public string PowerModel_TX { get; set; }
        public string PowerManufacturer_TX { get; set; }
        public string LightModel_TX { get; set; }
        public string LightManufacturer_TX { get; set; }
        public string GroupNumber_TX { get; set; }

        public int LightFunction_NR { get; set; }
        public int LightSource_NR { get; set; }
        public int LampType_NR { get; set; }
        public string PoleNumber_TX { get; set; }
        public string PoleManufacturers_TX { get; set; }
        public string FlangeSize_TX { get; set; }
        public string BasicFrameSize_TX { get; set; }
        public string CableType_NR { get; set; }
        public int PowerPhase_NR { get; set; }
        public float DimmingFactor_NR { get; set; }
        public int DefaultDimmingValue_NR { get; set; }
        public int PowerOnDimmingValue_NR { get; set; }
        public int MaximumDimmingValue_NR { get; set; }
        public int MinimumDimmingValue_NR { get; set; }
        public int PowerGroundingType_NR { get; set; }
        public float NorminalPower_NR { get; set; }
        public int LightColor_NR { get; set; }
        public int PoleHieght_NR { get; set; }
        public DateTime InstallationDate_DT { get; set; }
        public char Active_YN { get; set; }
        public char EnablePIRFunction_YN { get; set; }
        public int PIRLastTimeMinutesOfDimming_NR { get; set; }
        public int PIRRestoreTime_NR { get; set; }
        public int PIRGroupNumber_NR { get; set; }
        public int PIRIdleDimmingValue_NR { get; set; }
        public int PIRDimmingValue_NR { get; set; }
        public int PIRTTL_NR { get; set; }
        public long LightConfig_ID { get; set; }
        public long LightTimeControl_ID { get; set; }
        public float Longitude2 { get; set; }
        public float Latitude2 { get; set; }
        public int StorageInterval_NR { get; set; }
        public int SamplingInterval_NR { get; set; }
        public long PIRSensorPlan_ID { get; set; }
        public string LastUID { get; set; }
        public DateTime SampleDate { get; set; }
        public DateTime RecDateTime { get; set; }
        public string InfoLink { get; set; }
        public float TemperatureHigh_NR { get; set; }
        public float TemperatureLow_NR { get; set; }
        public int OID { get; set; }
        public int IsIot { get; set; }
        public string IMEI { get; set; }
        public string IMSI { get; set; }
        public string ICCID { get; set; }
        public string DeviceId { get; set; }
        public long Scheme_ID { get; set; }
        public int ChannelNumber { get; set; }
        public long RTCTimeDimmingPlan_ID { get; set; }
    }
}
