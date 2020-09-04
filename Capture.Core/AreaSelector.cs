namespace Capture.Core
{
    using System.Collections.Generic;
    using System.Drawing;
    using System.Linq;
    using System.Windows.Forms;

    public class AreaSelector : IAreaSelector
    {
        #region Propertires
        
        public IEnumerable<string> Devices => Screen.AllScreens.Select(s => s.DeviceName);

        #endregion Propertires

        #region Methods

        public Rectangle GetScreenAll()
        {
            var result = new Rectangle();

            Screen.AllScreens.ToList().ForEach(f => result = Rectangle.Union(result, f.Bounds));

            return result;
        }

        public Rectangle GetScreenArea()
        {
            // TODO Replace.
            using (var selected = new TopForm())
            {
                if (selected.ShowDialog() != DialogResult.OK || selected.w == 0 || selected.h == 0)
                    return GetScreenAll();

                // Hint: Должны быть кратны 2ум.
                if ((selected.w & 1) != 0)
                    selected.w += 1;

                if ((selected.h & 1) != 0)
                    selected.h += 1;

                return new Rectangle(selected.l, selected.t, selected.w, selected.h);
            }
        }

        public Rectangle GetScreenDevice(string deviceName)
        {
            return 
                string.IsNullOrWhiteSpace(deviceName) 
                    ? Rectangle.Empty 
                    : Screen.AllScreens.First(scr => scr.DeviceName.Equals(deviceName)).Bounds;
        }

        public Rectangle GetScreenWindow()
        {
            // TODO Impl.
            throw new System.NotImplementedException();
        }

        #endregion Methods
    }
}
