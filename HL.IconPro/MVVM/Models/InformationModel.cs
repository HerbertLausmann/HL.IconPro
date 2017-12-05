using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HL.MVVM;

namespace HL.IconPro.MVVM.Models
{
    class InformationModel : ModelBase
    {
        private string _name, _value;

        public InformationModel(string Name, string Value)
        {
            _name = Name;
            _value = Value;
        }

        public string Name
        {
            get
            {
                return _name;
            }
            set
            {
                SetField(ref _name, value);
            }
        }

        public string Value
        {
            get
            {
                return _value;
            }
            set
            {
                SetField(ref _value, value);
                OnPropertyChanged("ToString");
            }
        }

        public new string ToString
        {
            get
            {
                return Name + ": " + Value;
            }
        }

    }
}
