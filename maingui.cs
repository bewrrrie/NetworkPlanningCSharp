using System.Windows.Forms;
using System.Drawing;

namespace App
{
	public class Simple : Form
	{
		public Simple()
		{
			Text = "Simple";
			Size = new Size(250, 200);
			CenterToScreen();
		}

		static public void Main()
		{
			Application.Run(new Simple());
		}
	}
}