using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Appointments.AppointmentsProvider;
using Windows.Foundation;
using Windows.UI.Xaml.Controls;

namespace Dogfight_Arena.Objects
{
    public class Crate:GameObject
    {
        public CrateTypes CrateType { get; set; }
        public enum CrateTypes
        {
            HealthCrate,MissileCrate
        }
        public Crate(double x, double y, string fileName, Canvas field, double size, CrateTypes crateType) : base(x, y, fileName, field, size)
        {
            CrateType = crateType;
        }
        public void Remove()
        {
            _field.Children.Remove(_objectImage);
        }
        public override Rect Rect()
        {

            return new Rect(_x, _y, _objectImage.ActualWidth, _objectImage.ActualWidth);
        }
    }
}
