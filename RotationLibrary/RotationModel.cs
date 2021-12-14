using System.Collections.Generic;

namespace RotationLibrary
{
    public class RotationModel
    {
        public string RotationName { get; set; }
        public List<string> Rotation { get; set; } = new List<string>();

        public bool IsEmpty()
        {
            bool isEmpty = false;

            if (Rotation.Count == 0)
            {
                isEmpty = true;
            }

            return isEmpty;
        }

        public string GetNextUp()
        {
            if (Rotation.Count > 0)
            {
                return Rotation[0];
            }
            else
            {
                return null;
            }
        }

        public void AdvanceRotation()
        {
            if (Rotation.Count > 0)
            {
                string employeeWhoWent = Rotation[0];
                Rotation.RemoveAt(0);
                Rotation.Add(employeeWhoWent);
            }
        }
    }
}
