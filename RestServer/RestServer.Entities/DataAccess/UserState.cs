namespace RestServer.Entities.DataAccess
{
    using Cache;
    using Core;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel.DataAnnotations;
    using System.ComponentModel.DataAnnotations.Schema;
    using System.Data.Entity.Spatial;

    public enum UserState
    {
        None = 0,
        VerificationPending = 1,
        MobileVerified = 2,
        MobileAndEmailVerified = 3
    }
}
