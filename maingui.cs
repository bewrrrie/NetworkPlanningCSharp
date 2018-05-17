using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using System.Drawing;
using System.Threading;

using netplanlib;

namespace App
{
	public class Window : Form
	{
		private int WIDTH = 1009;
		private int HEIGHT = 600;

		private int TOP_MARGIN = 100;
		private int LEFT_MARGIN = 50;
		private int TEXT_BOX_WIDTH = 50;
		private int TEXT_BOX_HEIGHT = 23;

		private const string TITLE = "Network Planning";

		private int vertices = 18;
		private int textBoxCount;
		private long solutionNumber;
		private TextBox solutionBox;
		private WeightedDiGraph G;
		private bool loaded = false;

		private TabControl tabControl;
		private TabPage paramTab, solutionTab;

		public Window()
		{
			InitializeFields();
			InitializeComponent();
		}

		private void InitializeFields()
		{
			this.SuspendLayout();
			this.ClientSize = new System.Drawing.Size(WIDTH, HEIGHT);
			this.Text = TITLE;
			this.ResumeLayout(false);
			this.PerformLayout();
			this.FormBorderStyle = FormBorderStyle.FixedSingle;
			this.solutionBox = new TextBox();
		}

		private void InitializeComponent()
		{
			InitializeTabs();

			textBoxCount = 0;

			Label verticesNumberField = new Label();
			verticesNumberField.Text = " = " + vertices;
			verticesNumberField.Left = 3 * LEFT_MARGIN / 2 + 15 * TEXT_BOX_WIDTH / 10;
			verticesNumberField.Top = TOP_MARGIN / 4 + 4;
			verticesNumberField.Font = new Font("Georgia", 8);
			verticesNumberField.SetBounds
			(
				verticesNumberField.Location.X, verticesNumberField.Location.Y,
				TEXT_BOX_WIDTH, TEXT_BOX_HEIGHT
			);
			paramTab.Controls.Add(verticesNumberField);

			Label label = new Label();
			label.Left = LEFT_MARGIN;
			label.Top = TOP_MARGIN / 4;
			label.Font = new Font("Georgia", 8);
			label.Text = "Maximal number\nof vertices";
			label.SetBounds(label.Location.X, label.Location.Y, 128, 37);
			paramTab.Controls.Add(label);

			for (int i = 0; i < vertices * vertices; i++)
				AddTextBox();

			Button SolveButton = new Button();
			SolveButton.Height = 40;
			SolveButton.Width = 300;
			SolveButton.Left = WIDTH / 2 - 150;
			SolveButton.Top = (vertices + 5) * TEXT_BOX_HEIGHT;
			SolveButton.Text = "Solve";
			SolveButton.Font = new Font("Georgia", 16);
			SolveButton.Click += new EventHandler(SolveButton_Click);
			paramTab.Controls.Add(SolveButton);

			Button loadButton = new Button();
			loadButton.Left = 13 * LEFT_MARGIN;
			loadButton.Top = 2 * TOP_MARGIN / 4 ;
			loadButton.Font = new Font("Georgia", 10);
			loadButton.Click += new EventHandler(LoadButton_Click);
			loadButton.Text = "Load from file";
			loadButton.SetBounds(loadButton.Location.X, loadButton.Location.Y, 128, 22);
			paramTab.Controls.Add(loadButton);

			Label pathLabel = new Label();
			pathLabel.Left = 12 * LEFT_MARGIN;
			pathLabel.Top = TOP_MARGIN / 4;
			pathLabel.Font = new Font("Georgia", 8);
			pathLabel.Text = "Path: ";
			pathLabel.SetBounds(pathLabel.Location.X, pathLabel.Location.Y, 50, 37);
			paramTab.Controls.Add(pathLabel);
			
			TextBox pathBox = new TextBox();
			pathBox.Left = 13 * LEFT_MARGIN;
			pathBox.Top = TOP_MARGIN / 4;
			pathBox.Font = new Font("Georgia", 8);
			pathBox.Name = "path";
			pathBox.SetBounds(pathBox.Location.X, pathBox.Location.Y, 275, 20);
			paramTab.Controls.Add(pathBox);

			solutionBox.Font = new Font("Georgia", 12);
			solutionBox.ScrollBars = ScrollBars.Both;
			solutionBox.Multiline = true;
			solutionBox.ReadOnly = true;
			solutionBox.SetBounds(solutionBox.Location.X, solutionBox.Location.Y, WIDTH, HEIGHT);
			solutionTab.Controls.Add(solutionBox);
		}

		private void InitializeTabs()
		{
			tabControl = new TabControl();

			paramTab = new TabPage();
			solutionTab = new TabPage();
			paramTab.Text = "Parameters";
			solutionTab.Text = "Solution";

			tabControl.Controls.AddRange(new Control[] {paramTab, solutionTab});
			tabControl.Size = new Size(WIDTH, HEIGHT);
			Controls.AddRange(new Control[] {tabControl});
		}

		private void SolveButton_Click(object sender, EventArgs e)
		{
			List<Arrow> cp;
			var thread = new Thread
			(
				() =>
				{
					if (!loaded)
					{
						List<Arrow> arrows = new List<Arrow>();

						for (int i = 1; i <= vertices; i++)
							for (int j = 1; j <= vertices; j++)
							{
								double weight = Convert.ToDouble(Controls.Find(i + "," + j, true)[0].Text);

								if (weight >= 0)
								arrows.Add(new Arrow(i, j, weight));
							}

						G = new WeightedDiGraph(arrows);
					}

					cp = CPM.GetCriticalPath(G);

					solutionBox.Text += "Solution #" + solutionNumber + ":\nCritical path: " + cp[0].GetBegin();

					double summWeight = 0;
					foreach (Arrow a in cp)
					{
						summWeight += a.GetWeight();
						solutionBox.Text += ", " + a.GetEnd();
					}
					solutionBox.Text += ";\nSumm weight = " + summWeight + ".\n\n";
					solutionNumber++;
				}
			);

			thread.Start();
			thread.Join();
		}

		private void LoadButton_Click(object sender, EventArgs e)
		{
			G = GraphReader.read(Controls.Find("path", true)[0].Text);
			int n = G.GetVertices().Count();

			for (int i = 1; i <= n; i++)
				for (int j = 1; j <= n; j++)
					Controls.Find(i + "," + j, true)[0].Text = G.GetArrowWeight(i, j) + "";

			loaded = true;
		}

		private void TextBox_Leave(object sender, EventArgs e)
		{
			loaded = false;
		}

		private void AddTextBox()
		{
			TextBox box = new TextBox();

			box.Text = "-1";
			box.AcceptsReturn = true;
			box.AcceptsTab = true;
			box.Name = (textBoxCount / vertices + 1) + "," + (textBoxCount % vertices + 1);
			box.Leave += new EventHandler(TextBox_Leave);
			box.Left = LEFT_MARGIN + TEXT_BOX_WIDTH * (textBoxCount % vertices);
			box.Top = TOP_MARGIN + TEXT_BOX_HEIGHT * (textBoxCount / vertices);
			box.SetBounds(box.Location.X, box.Location.Y, TEXT_BOX_WIDTH, TEXT_BOX_HEIGHT);
			box.MaxLength = 10;

			if (textBoxCount < vertices)
			{
				Label label = new Label();

				label.Text = 1 + textBoxCount + "";
				label.Left = box.Left + 5;
				label.Top = TOP_MARGIN - 15;
				label.Font = new Font("Georgia", 8);
				label.SetBounds(label.Location.X, label.Location.Y, 15, 15);
				paramTab.Controls.Add(label);
			}
			if ((textBoxCount - 1) % vertices == 0)
			{
				Label label = new Label();
				label.Text = 1 + textBoxCount / vertices + "";
				label.Left = LEFT_MARGIN - 15;
				label.Top = box.Top;
				label.Font = new Font("Georgia", 8);
				label.SetBounds(label.Location.X, label.Location.Y, 15, 15);
				paramTab.Controls.Add(label);
			}

			paramTab.Controls.Add(box);
			textBoxCount++;
		}

		static void Main()
		{
			Application.EnableVisualStyles();
			Application.SetCompatibleTextRenderingDefault(false);
			Application.Run(new Window());
		}

	}
}