using System;

//TODO: Add other using statements here

namespace com.Sconit.Entity.ISI
{

    public partial class TaskSubType 
    {
        #region Non O/R Mapping Properties

        public string CodeDescription
        {
            get
            {
                return this.Code + "[" + this.Description + "]";
            }
        }

        private string _name = string.Empty;
        public string Name
        {
            get
            {
                if (_name == string.Empty)
                {
                    _name = this.Description;
                }
                return _name;
            }
            set
            {
                _name = value;
            }

        }

        public string[] AssignUpLevel
        {
            get
            {
                if (string.IsNullOrEmpty(this.AssignUpUser))
                {
                    return new string[0];
                }
                else
                {
                    return this.AssignUpUser.Substring(1, this.AssignUpUser.Length - 2).Split('|');
                }
            }
        }

        public string AssignUpLevelCount
        {
            get
            {
                int count = this.AssignUpLevel.Length;
                if (count == 0)
                {
                    return string.Empty;
                }
                else
                {
                    return count.ToString();
                }
            }
        }

        public string[] StartUpLevel
        {
            get
            {
                if (string.IsNullOrEmpty(this.StartUpUser))
                {
                    return new string[0];
                }
                else
                {
                    return this.StartUpUser.Substring(1, this.StartUpUser.Length - 2).Split('|');
                }
            }
        }

        public string StartUpLevelCount
        {
            get
            {
                int count = this.StartUpLevel.Length;
                if (count == 0)
                {
                    return string.Empty;
                }
                else
                {
                    return count.ToString();
                }
            }
        }

        public string[] CloseUpLevel
        {
            get
            {
                if (string.IsNullOrEmpty(this.CloseUpUser))
                {
                    return new string[0];
                }
                else
                {
                    return this.CloseUpUser.Substring(1, this.CloseUpUser.Length - 2).Split('|');
                }
            }
        }

        public string CloseUpLevelCount
        {
            get
            {
                int count = this.CloseUpLevel.Length;
                if (count == 0)
                {
                    return string.Empty;
                }
                else
                {
                    return count.ToString();
                }
            }
        }

        #endregion
    }
}