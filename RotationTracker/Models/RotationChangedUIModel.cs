using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RotationTracker.Models
{
    public class RotationChangedUIModel
    {
        private bool _rotationHasChanged = false;

        public event EventHandler<string> RotationChanged;

        public bool RotationHasChanged
        {
            get { return _rotationHasChanged; }
            set
            {
                if (value != _rotationHasChanged)
                {
                    RotationChanged?.Invoke(this, $"The rotation has changed.{Environment.NewLine}" +
                    $"On Calendar has been cleared for all employees in rotation.{Environment.NewLine}" +
                    "Calendar will need to be checked and On Calendar re-marked.");
                }

                _rotationHasChanged = value;
            }
        }
    }
}
