using System.Collections.Generic;
using System.Text;
using Unity.UIWidgets.foundation;
using Unity.UIWidgets.gestures;
using Unity.UIWidgets.ui;

namespace Unity.UIWidgets.painting {
    abstract class PlaceholderSpan : InlineSpan {
        public PlaceholderSpan(
            ui.TextBaseline baseline,
            TextStyle style,
            ui.PlaceholderAlignment alignment = ui.PlaceholderAlignment.bottom,
            HoverRecognizer hoverRecognizer = null
        ) : base(style: style, hoverRecognizer: hoverRecognizer) {
            this.baseline = baseline;
            this.alignment = alignment;
        }

        public ui.PlaceholderAlignment alignment;

        public ui.TextBaseline baseline;

        protected internal override void computeToPlainText(
            StringBuilder buffer,
            bool includeSemanticsLabels = true,
            bool includePlaceholders = true
        ) {
            if (includePlaceholders) {
                buffer.Append('\uFFFC');
            }
        }

        protected internal override void computeSemanticsInformation(List<InlineSpanSemanticsInformation> collector) {
            collector.Add(InlineSpanSemanticsInformation.placeholder);
        }

        public override void debugFillProperties(DiagnosticPropertiesBuilder properties) {
            base.debugFillProperties(properties);

            properties.add(new EnumProperty<ui.PlaceholderAlignment>("alignment", this.alignment, defaultValue: null));
            properties.add(new EnumProperty<TextBaseline>("baseline", this.baseline, defaultValue: null));
        }
    }
}