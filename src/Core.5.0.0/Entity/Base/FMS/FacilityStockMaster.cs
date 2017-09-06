using System;
using System.Collections;
using System.Collections.Generic;
using com.Sconit.Entity;

//TODO: Add other using statements here

namespace com.Sconit.Entity.FMS
{
    [Serializable]
    public partial class FacilityStockMaster : EntityBase
    {
        #region O/R Mapping Properties

        private string _stNo;
        public string StNo
        {
            get
            {
                return _stNo;
            }
            set
            {
                _stNo = value;
            }
        }
        private DateTime _effDate;
        public DateTime EffDate
        {
            get
            {
                return _effDate;
            }
            set
            {
                _effDate = value;
            }
        }
        private string _createUser;
        public string CreateUser
        {
            get
            {
                return _createUser;
            }
            set
            {
                _createUser = value;
            }
        }
        private DateTime _createDate;
        public DateTime CreateDate
        {
            get
            {
                return _createDate;
            }
            set
            {
                _createDate = value;
            }
        }
        private string _lastModifyUser;
        public string LastModifyUser
        {
            get
            {
                return _lastModifyUser;
            }
            set
            {
                _lastModifyUser = value;
            }
        }
        private DateTime _lastModifyDate;
        public DateTime LastModifyDate
        {
            get
            {
                return _lastModifyDate;
            }
            set
            {
                _lastModifyDate = value;
            }
        }
        private string _chargeOrg;
        public string ChargeOrg
        {
            get
            {
                return _chargeOrg;
            }
            set
            {
                _chargeOrg = value;
            }
        }
        private string _chargePerson;
        public string ChargePerson
        {
            get
            {
                return _chargePerson;
            }
            set
            {
                _chargePerson = value;
            }
        }
        private string _chargePersonName;
        public string ChargePersonName
        {
            get
            {
                return _chargePersonName;
            }
            set
            {
                _chargePersonName = value;
            }
        }
        private string _chargeSite;
        public string ChargeSite
        {
            get
            {
                return _chargeSite;
            }
            set
            {
                _chargeSite = value;
            }
        }
        private string _facilityCategory;
        public string FacilityCategory
        {
            get
            {
                return _facilityCategory;
            }
            set
            {
                _facilityCategory = value;
            }
        }
        private string _status;
        public string Status
        {
            get
            {
                return _status;
            }
            set
            {
                _status = value;
            }
        }
        private string _releaseUser;
        public string ReleaseUser
        {
            get
            {
                return _releaseUser;
            }
            set
            {
                _releaseUser = value;
            }
        }
        private DateTime? _releaseDate;
        public DateTime? ReleaseDate
        {
            get
            {
                return _releaseDate;
            }
            set
            {
                _releaseDate = value;
            }
        }
        private string _cancelUser;
        public string CancelUser
        {
            get
            {
                return _cancelUser;
            }
            set
            {
                _cancelUser = value;
            }
        }
        private DateTime? _cancelDate;
        public DateTime? CancelDate
        {
            get
            {
                return _cancelDate;
            }
            set
            {
                _cancelDate = value;
            }
        }
        private string _closeUser;
        public string CloseUser
        {
            get
            {
                return _closeUser;
            }
            set
            {
                _closeUser = value;
            }
        }
        private DateTime? _closeDate;
        public DateTime? CloseDate
        {
            get
            {
                return _closeDate;
            }
            set
            {
                _closeDate = value;
            }
        }
        private string _startUser;
        public string StartUser
        {
            get
            {
                return _startUser;
            }
            set
            {
                _startUser = value;
            }
        }
        private DateTime? _startDate;
        public DateTime? StartDate
        {
            get
            {
                return _startDate;
            }
            set
            {
                _startDate = value;
            }
        }
        private string _completeUser;
        public string CompleteUser
        {
            get
            {
                return _completeUser;
            }
            set
            {
                _completeUser = value;
            }
        }
        private DateTime? _completeDate;
        public DateTime? CompleteDate
        {
            get
            {
                return _completeDate;
            }
            set
            {
                _completeDate = value;
            }
        }
        private string _assetNo;
        public string AssetNo
        {
            get
            {
                return _assetNo;
            }
            set
            {
                _assetNo = value;
            }
        }
        #endregion

        public override int GetHashCode()
        {
            if (StNo != null)
            {
                return StNo.GetHashCode();
            }
            else
            {
                return base.GetHashCode();
            }
        }

        public override bool Equals(object obj)
        {
            FacilityStockMaster another = obj as FacilityStockMaster;

            if (another == null)
            {
                return false;
            }
            else
            {
                return (this.StNo == another.StNo);
            }
        }
    }

}
