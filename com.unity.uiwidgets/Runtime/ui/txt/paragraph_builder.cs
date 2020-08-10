using System.Collections.Generic;
using System.Text;
using Unity.UIWidgets.foundation;

namespace Unity.UIWidgets.ui {
    public class ParagraphBuilder {
        StringBuilder _text = new StringBuilder();
        ParagraphStyle _paragraphStyle;
        StyledRuns _runs = new StyledRuns();
        List<int> _styleStack = new List<int>();
        int _paragraph_style_index;

        int _placeholderCount;

        public int placeholderCount {
            get { return this._placeholderCount; }
        }
        
        public List<float> placeholderScales {
            get { return this._placeholderScales; }
        }

        List<float> _placeholderScales = new List<float>();

        public ParagraphBuilder(ParagraphStyle style) {
            this.setParagraphStyle(style);
        }

        public Paragraph build() {
            this._runs.endRunIfNeeded(this._text.Length);
            var paragraph = Paragraph.create();
            paragraph.setText(this._text.ToString(), this._runs);
            paragraph.setParagraphStyle(this._paragraphStyle);
            return paragraph;
        }

        public void pushStyle(painting.TextStyle style, float textScaleFactor) {
            var newStyle = TextStyle.applyStyle(this.peekStyle(), style, textScaleFactor: textScaleFactor);
            var styleIndex = this._runs.addStyle(newStyle);
            this._styleStack.Add(styleIndex);
            this._runs.startRun(styleIndex, this._text.Length);
        }

        internal void pushStyle(TextStyle style) {
            var styleIndex = this._runs.addStyle(style);
            this._styleStack.Add(styleIndex);
            this._runs.startRun(styleIndex, this._text.Length);
        }

        public void pop() {
            var lastIndex = this._styleStack.Count - 1;
            if (lastIndex < 0) {
                return;
            }

            this._styleStack.RemoveAt(lastIndex);
            this._runs.startRun(this.peekStyleIndex(), this._text.Length);
        }

        public void addText(string text) {
            this._text.Append(text);
        }

        public void addPlaceholder(
            float width,
            float height,
            PlaceholderAlignment alignment,
            float? baselineOffset,
            TextBaseline? baseline,
            float? scale = 1.0f
        ) {
            D.assert((alignment != PlaceholderAlignment.aboveBaseline && alignment != PlaceholderAlignment.belowBaseline && alignment != PlaceholderAlignment.baseline) || baseline != null);
            baselineOffset = baselineOffset ?? height;
            this._addPlaceholder(width * scale, height * scale, (int) alignment,
                baselineOffset  * scale, (int)baseline);
            this._placeholderCount++;
            this._placeholderScales.Add(scale ?? 0);
        }

        string _addPlaceholder(float? width, float? height, int alignment, float? baselineOffset, int baseline) {
            // TODO native 'ParagraphBuilder_addPlaceholder';
            return "";
        }


        internal TextStyle peekStyle() {
            return this._runs.getStyle(this.peekStyleIndex());
        }


        public int peekStyleIndex() {
            int count = this._styleStack.Count;
            if (count > 0) {
                return this._styleStack[count - 1];
            }

            return this._paragraph_style_index;
        }

        void setParagraphStyle(ParagraphStyle style) {
            this._paragraphStyle = style;
            this._paragraph_style_index = this._runs.addStyle(style.getTextStyle());
            this._runs.startRun(this._paragraph_style_index, this._text.Length);
        }
    }
}