using NbIotCmd.Entity;
using NbIotCmd.Helper;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace NbIotCmd.Service
{
    public class TNLService
    {
        public TNLService() { }


        public async Task<int> SaveLightInfo(TNL_TunnelLight tunnelLight)
        {
            try
            {
                using (var context = new EFContext())
                {
                    //var light = (from d in context.TNL_TunnelLights
                    //             where d.TunnelLight_ID == 1000044L && d.DeviceId != null
                    //             select d).FirstOrDefault();
                    TNL_TunnelLight light = null;
                    if (light == null)
                    {
                        light = new TNL_TunnelLight();
                        light.TunnelLight_ID = await DataBaseHelper.GetKey("TNL_TunnelLight", "TunnelLight_ID");
                    }
                    light.TunnelLight_ID = -1;
                    light.TunnelSection_ID = -1;
                    light.TunnelGateway_ID = -1;
                    light.Tunnel_ID = -1;
                    light.LightPhysicalAddress_TX = string.Empty;
                    light.LightLocationNumber_NR = -1;
                    light.LightUsage_NR = -1;
                    light.GroupNumber_TX = string.Empty;
                    light.LightFunction_NR = -1;
                    light.LightSource_NR = -1;
                    light.LampType_NR = -1;
                    light.DimmingFactor_NR = 1;
                    light.DefaultDimmingValue_NR = 1;
                    light.PowerOnDimmingValue_NR = 1;
                    light.MaximumDimmingValue_NR = 1;
                    light.MinimumDimmingValue_NR = 1;

                    light.Active_YN = '0';
                    light.EnablePIRFunction_YN = '0';
                    light.PIRLastTimeMinutesOfDimming_NR = -1;
                    light.PIRRestoreTime_NR = -1;
                    light.PIRGroupNumber_NR = -1;
                    light.PIRIdleDimmingValue_NR = -1;
                    light.PIRDimmingValue_NR = -1;
                    light.PIRTTL_NR = -1;
                    light.LightConfig_ID = -1;
                    light.LightTimeControl_ID = -1;

                    light.PIRSensorPlan_ID = -1;

                    context.Add(light);
                    return await context.SaveChangesAsync();
                }
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}
