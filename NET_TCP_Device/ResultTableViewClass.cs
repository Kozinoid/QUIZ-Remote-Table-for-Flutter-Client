using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NET_TCP_Device
{
    public partial class ResultTableViewClass : UserControl
    {
        private ResultTableDataClass dataTable = null;

        //---------------------------------  Пропорции  ------------------------------------
        float indexInListHeaderWidthWeight = 10.0f;
        float teamHeaderWidthWeight = 60.0f;
        float scoreHeaderWidthWeight = 30.0f;
        float headerHeightWeight = 10.0f;

        float teamHeightWeight = 7.0f;
        float teamHeightWeightMin = 6.0f;
        float teamHeightWeightMax = 10.0f;

        float x0 = 0;
        float y0 = 0;
        float indexInListHeaderWidth = 0;
        float teamHeaderWidth = 0;
        float scoreHeaderWidth = 0;
        float headerHeight = 0;
        float teamHeight = 0;

        //------------------------------------  Цвета  -------------------------------------
        Color backColor = Color.Black;
        Color lineColor = Color.BlueViolet;
        Color headerColor = Color.BlueViolet;
        Color fieldUpColor = Color.FromArgb(0x4C, 0xB0, 0x50);
        Color fieldMidColor = Color.FromArgb(0x21, 0x96, 0xF3);
        Color fieldDownColor = Color.FromArgb(0xF4, 0x42, 0x36);
        Color textColor = Color.White;
        Color shadowColor = Color.Black;

        //------------------------------------  Шрифты  ------------------------------------
        StringFormat centerStringFormat;
        StringFormat leftStringFormat;
        Font headerFont = new Font("Times New Roman", 16f);
        Font teamFont = new Font("Times New Roman", 16f);
        Font allTextFont = new Font("Times New Roman", 16f);

        public Font AllTextFont
        {
            set 
            {
                allTextFont = value;
                Console.WriteLine(headerFont.FontFamily.ToString());
                Refresh();
                
            }
            get
            {
                return allTextFont;
            }
        }
        //----------------------------------------------------------------------------------

        public ResultTableDataClass DataTable
        {
            set
            {
                if (dataTable == null)
                {
                    if (value != null)
                    {
                        dataTable = value;
                        dataTable.onTableChanged += dataTable_onTableChanged;
                    }
                }
                else
                {
                    if (value != null)
                    {
                        dataTable.onTableChanged -= dataTable_onTableChanged;
                        dataTable = value;
                        dataTable.onTableChanged += dataTable_onTableChanged;
                    }
                    else
                    {
                        dataTable.onTableChanged -= dataTable_onTableChanged;
                        dataTable = value;
                    }
                }
            }
        }

        void dataTable_onTableChanged(object sender, EventArgs e)
        {
            ColorSort();
            Refresh();
        }

        private void ColorSort()
        {
            if (dataTable.Count > 0)
            {
                int max = dataTable[0].TeamScore;
                int min = max;

                int i = 1;
                while (i < dataTable.Count)
                {
                    if (dataTable[i].TeamScore > max) max = dataTable[i].TeamScore;
                    if (dataTable[i].TeamScore < min) min = dataTable[i].TeamScore;
                    i++;
                }

                for (i = 0; i < dataTable.Count; i++)
                {
                    if (dataTable[i].TeamScore == max) dataTable[i].TeamColor = fieldUpColor;
                    else if ((min < max) && (dataTable[i].TeamScore == min)) dataTable[i].TeamColor = fieldDownColor;
                    else dataTable[i].TeamColor = fieldMidColor;
                }
            }
        }

        public ResultTableViewClass()
        {
            InitializeComponent();

            centerStringFormat = new StringFormat();
            centerStringFormat.LineAlignment = StringAlignment.Center;
            centerStringFormat.Alignment = StringAlignment.Center;

            leftStringFormat = new StringFormat();
            leftStringFormat.LineAlignment = StringAlignment.Center;
            leftStringFormat.Alignment = StringAlignment.Near;
        }

        private void ResultTableViewClass_Resize(object sender, EventArgs e)
        {
            Refresh();
        }

        private void ResultTableViewClass_Paint(object sender, PaintEventArgs e)
        {
            if (dataTable != null)
            {
                DrawTabl(e.Graphics, this.ClientRectangle);
            }
        }

        private void DrawTabl(Graphics gr, Rectangle rect)
        {
            CalculateLayouts(rect);

            gr.Clear(backColor);
            DrawHeader(gr);
            if (dataTable.Count > 0)
            {
                SizeF maxSize = MesureTeamString(gr, 0);
                for (int i = 1; i < dataTable.Count; i++)
                {
                    SizeF size = MesureTeamString(gr, i);
                    if (size.Width > maxSize.Width)
                    {
                        maxSize = size;
                    }
                }

                float xTeamName = x0 + indexInListHeaderWidth + (teamHeaderWidth - maxSize.Width) / 2;

                for (int i = 0; i < dataTable.Count; i++)
                {
                    DrawTeam(gr, i, xTeamName);
                }
            }
        }

        private void DrawHeader(Graphics gr)
        {
            gr.FillRectangle(new SolidBrush(headerColor), x0, y0, indexInListHeaderWidth, headerHeight);
            gr.FillRectangle(new SolidBrush(headerColor), x0 + indexInListHeaderWidth, y0, teamHeaderWidth, headerHeight);
            gr.FillRectangle(new SolidBrush(headerColor), x0 + indexInListHeaderWidth + teamHeaderWidth, y0, scoreHeaderWidth, headerHeight);

            gr.DrawRectangle(new Pen(lineColor), x0, y0, indexInListHeaderWidth, headerHeight);
            gr.DrawRectangle(new Pen(lineColor), x0 + indexInListHeaderWidth, y0, teamHeaderWidth, headerHeight);
            gr.DrawRectangle(new Pen(lineColor), x0 + indexInListHeaderWidth + teamHeaderWidth, y0, scoreHeaderWidth, headerHeight);

            gr.DrawString("№", headerFont, new SolidBrush(shadowColor), new RectangleF(x0 + 1.0f, y0 + 1.0f, indexInListHeaderWidth, headerHeight), centerStringFormat);
            gr.DrawString("Команда", headerFont, new SolidBrush(shadowColor), new RectangleF(x0 + indexInListHeaderWidth + 1.0f, y0 + 1.0f, teamHeaderWidth, headerHeight), centerStringFormat);
            gr.DrawString("Результат", headerFont, new SolidBrush(shadowColor), new RectangleF(x0 + indexInListHeaderWidth + teamHeaderWidth + 1.0f, y0 + 1.0f, scoreHeaderWidth, headerHeight), centerStringFormat);

            gr.DrawString("№", headerFont, new SolidBrush(textColor), new RectangleF(x0, y0, indexInListHeaderWidth, headerHeight), centerStringFormat);
            gr.DrawString("Команда", headerFont, new SolidBrush(textColor), new RectangleF(x0 + indexInListHeaderWidth, y0, teamHeaderWidth, headerHeight), centerStringFormat);
            gr.DrawString("Результат", headerFont, new SolidBrush(textColor), new RectangleF(x0 + indexInListHeaderWidth + teamHeaderWidth, y0, scoreHeaderWidth, headerHeight), centerStringFormat);
        }

        private SizeF MesureTeamString(Graphics gr, int index)
        {
            return gr.MeasureString(dataTable[index].TeamName, teamFont);
        }

        private void DrawTeam(Graphics gr, int index, float teamX)
        {
            float yT = y0 + headerHeight + teamHeight * index;

            gr.FillRectangle(new SolidBrush(dataTable[index].TeamColor), x0, yT, indexInListHeaderWidth, teamHeight);
            gr.FillRectangle(new SolidBrush(dataTable[index].TeamColor), x0 + indexInListHeaderWidth, yT, teamHeaderWidth, teamHeight);
            gr.FillRectangle(new SolidBrush(dataTable[index].TeamColor), x0 + indexInListHeaderWidth + teamHeaderWidth, yT, scoreHeaderWidth, teamHeight);

            gr.DrawRectangle(new Pen(lineColor), x0, yT, indexInListHeaderWidth, teamHeight);
            gr.DrawRectangle(new Pen(lineColor), x0 + indexInListHeaderWidth, yT, teamHeaderWidth, teamHeight);
            gr.DrawRectangle(new Pen(lineColor), x0 + indexInListHeaderWidth + teamHeaderWidth, yT, scoreHeaderWidth, teamHeight);

            gr.DrawString((index + 1).ToString(), teamFont, new SolidBrush(shadowColor), new RectangleF(x0 + 1.0f, yT + 1.0f, indexInListHeaderWidth, teamHeight), centerStringFormat);
            gr.DrawString(dataTable[index].TeamName, teamFont, new SolidBrush(shadowColor), new RectangleF(teamX + 1.0f, yT + 1.0f, teamHeaderWidth, teamHeight), leftStringFormat);
            gr.DrawString(dataTable[index].TeamScore.ToString(), teamFont, new SolidBrush(shadowColor), new RectangleF(x0 + indexInListHeaderWidth + teamHeaderWidth + 1.0f, yT + 1.0f, scoreHeaderWidth, teamHeight), centerStringFormat);

            gr.DrawString((index + 1).ToString(), teamFont, new SolidBrush(textColor), new RectangleF(x0, yT, indexInListHeaderWidth, teamHeight), centerStringFormat);
            gr.DrawString(dataTable[index].TeamName, teamFont, new SolidBrush(textColor), new RectangleF(teamX, yT, teamHeaderWidth, teamHeight), leftStringFormat);
            gr.DrawString(dataTable[index].TeamScore.ToString(), teamFont, new SolidBrush(textColor), new RectangleF(x0 + indexInListHeaderWidth + teamHeaderWidth, yT, scoreHeaderWidth, teamHeight), centerStringFormat);
        }

        private void CalculateLayouts(Rectangle rect)
        {
            if (dataTable.Count == 0) teamHeightWeight = teamHeightWeightMax;
            else
            {
                teamHeightWeight = (100.0f - headerHeightWeight) / dataTable.Count;
                if (teamHeightWeight < teamHeightWeightMin) teamHeightWeight = teamHeightWeightMin;
                if (teamHeightWeight > teamHeightWeightMax) teamHeightWeight = teamHeightWeightMax;
            }

            x0 = (float)rect.X + WidthPercentConvertToFloat(rect, (100.0f - (indexInListHeaderWidthWeight + teamHeaderWidthWeight + scoreHeaderWidthWeight)) / 2);
            if (x0 < 0) x0 = 0;
            y0 = (float)rect.Y + HeightPercentConvertToFloat(rect, (100.0f - (headerHeightWeight + AllTeamsPercentHeight())) / 2);
            if (y0 < 0) y0 = 0;

            indexInListHeaderWidth = WidthPercentConvertToFloat(rect, indexInListHeaderWidthWeight);
            teamHeaderWidth = WidthPercentConvertToFloat(rect, teamHeaderWidthWeight);
            scoreHeaderWidth = WidthPercentConvertToFloat(rect, scoreHeaderWidthWeight);

            headerHeight = HeightPercentConvertToFloat(rect, headerHeightWeight);
            teamHeight = HeightPercentConvertToFloat(rect, teamHeightWeight);

            headerFont = new Font(allTextFont.FontFamily, headerHeight * 0.6f, allTextFont.Style);
            teamFont = new Font(allTextFont.FontFamily, teamHeight * 0.6f, allTextFont.Style);
        }

        private float WidthPercentConvertToFloat(Rectangle rect, float percent)
        {
            return (float)rect.Width * (percent / 100.0f);
        }

        private float HeightPercentConvertToFloat(Rectangle rect, float percent)
        {
            return (float)rect.Height * (percent / 100.0f);
        }

        private float AllTeamsPercentHeight()
        {
            return dataTable.Count * teamHeightWeight;
        }

        public Image DrawToImage(Size sz)
        {
            Bitmap bmp = new Bitmap(sz.Width, sz.Height);
            Image img = Image.FromHbitmap(bmp.GetHbitmap());
            DrawTabl(Graphics.FromImage(img), new Rectangle(new Point(0,0), sz));
            return img;
        }
    }
}
