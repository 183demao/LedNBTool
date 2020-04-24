using Led.Tools;
using System;
using System.Collections.Generic;
using System.Text;

namespace Led.SingleLight
{
    public class SingleLightDimmingScheme
    {
        public string TimeZoneId { get; set; }

        public string DeviceNumber { get; set; }

        public int BaseUtcOffset { get; set; }

        public string Address { get; set; }

        public double Lon { get; set; }

        public double Lat { get; set; }


        #region 解包辅助属性

        public int Mode { get; set; }

        public DateTime SampLightDateTime { get; set; }

        #endregion


        public List<LightSchemeItem> SchemeItems { get; set; }

        public DayLightRule RuleForDayLight { get; set; }

        public SingleLightDimmingScheme()
        {
            RuleForDayLight = new DayLightRule();
            SchemeItems = new List<LightSchemeItem>();
        }

        public void CalcDayLightRule()
        {
            try
            {
                var zone = TimeZoneInfo.FindSystemTimeZoneById(TimeZoneId);

                this.BaseUtcOffset = zone.BaseUtcOffset.Hours;

                this.RuleForDayLight.IsFixedDateRule = zone.SupportsDaylightSavingTime;

                if (zone.SupportsDaylightSavingTime == false)
                {
                    return;
                }

                var rules = zone.GetAdjustmentRules();

                if (rules.Length == 0)
                {
                    return;
                }
                var rule = rules[rules.Length - 1];



                this.RuleForDayLight.StartMonth = rule.DateStart.Month;
                this.RuleForDayLight.StartWeekofMonth = (int)rule.DateStart.DayOfWeek;
                this.RuleForDayLight.StartDay = rule.DateStart.Day;
                this.RuleForDayLight.StartDayOfWeek = (int)rule.DateStart.DayOfWeek;

                this.RuleForDayLight.EndMonth = rule.DateEnd.Month;
                this.RuleForDayLight.EndWeekofMonth = (int)rule.DateEnd.DayOfWeek;
                this.RuleForDayLight.EndDay = rule.DateEnd.Day;
                this.RuleForDayLight.EndDayOfWeek = (int)rule.DateEnd.DayOfWeek;

                this.RuleForDayLight.DaylightDelta = rule.DaylightDelta.Hours;

            }
            catch (Exception ex)
            {
                Console.Write(ex.Message);
            }

        }

        public bool IsValidLonLat()
        {
            return MapHelper.IsValidLonLat(Lon, Lat);
        }

        public class LightSchemeItem
        {
            public int BaseUtcOffset { get; set; }

            public long Item_ID { get; set; }

            public int Item_Mode { get; set; }

            public int Item_Week { get; set; }

            public int Item_Time { get; set; }

            public int Item_Channel { get; set; }

            public int Item_DimmingValue { get; set; }

            public long TimeDimmingPlan_ID { get; set; }


            public string ItemTimeString
            {
                get
                {
                    return TimeHelper.MinutesFromZero2TimeString(Item_Time);

                }
            }
        }

        public class DayLightRule
        {
            public bool IsFixedDateRule { get; set; }

            public int StartMonth { get; set; }

            public int StartDay { get; set; }

            public int StartWeekofMonth { get; set; }

            public int StartDayOfWeek { get; set; }

            public int EndMonth { get; set; }

            public int EndDay { get; set; }

            public int EndWeekofMonth { get; set; }

            public int EndDayOfWeek { get; set; }

            public int DaylightDelta { get; set; }

        }
    }
}
