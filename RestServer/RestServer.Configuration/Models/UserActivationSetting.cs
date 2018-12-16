using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestServer.Configuration.Models
{
    public class UserActivationSetting
    {
        public int MaxUserActivationExpiryPeriodInMinutes { get; set; }

        public int MaxOtpGenerationAttemptWindowThresholdCount { get; set; }

        public int UserActivationDelayPostMaxOtpGenerationAttemptInMinutes { get; set; }

        public int ActivationCodeMinValue { get; set; }
        
        public int ActivationCodeMaxValue { get; set; }

        public int MaxTotalOtpGenerationThresholdCount { get; set; }
    }
}
