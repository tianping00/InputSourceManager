using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Animation;

namespace InputSourceManager.Windows
{
	public partial class IndicatorWindow : Window
	{
		private readonly DoubleAnimation _fadeIn = new DoubleAnimation(0, 1, TimeSpan.FromMilliseconds(120));
		private readonly DoubleAnimation _fadeOut = new DoubleAnimation(1, 0, TimeSpan.FromMilliseconds(360));

		public IndicatorWindow()
		{
			InitializeComponent();
			Opacity = 0;
			PositionToBottomRight();
		}

		public void SetText(string text)
		{
			TxtIndicator.Text = text;
			PositionToBottomRight();
		}

		public async void ShowTemporarily(int ms = 1200)
		{
			Show();
			BeginAnimation(OpacityProperty, _fadeIn);
			await Task.Delay(ms);
			BeginAnimation(OpacityProperty, _fadeOut);
		}

		private void PositionToBottomRight()
		{
			var work = SystemParameters.WorkArea;
			Left = work.Right - Width - 24;
			Top = work.Bottom - Height - 24;
		}
	}
}





