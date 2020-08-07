using System.Collections.Generic;
using Unity.UIWidgets.foundation;
using Unity.UIWidgets.painting;
using Unity.UIWidgets.ui;
using TextStyle = Unity.UIWidgets.painting.TextStyle;

namespace Unity.UIWidgets.widgets {
    class WidgetSpan : PlaceholderSpan {
        public WidgetSpan(
            Widget child,
            ui.TextBaseline baseline,
            TextStyle style,
            ui.PlaceholderAlignment alignment = ui.PlaceholderAlignment.bottom
        ) : base(
            alignment: alignment,
            baseline: baseline,
            style: style
        ) {
            D.assert(child != null);
            D.assert(baseline != null || !(
                (alignment == ui.PlaceholderAlignment.aboveBaseline) ||
                (alignment == ui.PlaceholderAlignment.belowBaseline) ||
                (alignment == ui.PlaceholderAlignment.baseline)
            ));
            this.child = child;
        }

        Widget child;

        public override void build(
            ui.ParagraphBuilder builder,
            List<PlaceholderDimensions> dimensions,
            float textScaleFactor = 1.0f) {
            D.assert(this.debugAssertIsValid());
            D.assert(dimensions != null);
            bool hasStyle = this.style != null;
            if (hasStyle) {
                builder.pushStyle(this.style.getTextStyle(textScaleFactor: textScaleFactor));
            }

            D.assert(builder.placeholderCount < dimensions.Count);
            PlaceholderDimensions currentDimensions = dimensions[builder.placeholderCount];
            builder.addPlaceholder(
                currentDimensions.size.width,
                currentDimensions.size.height,
                this.alignment,
                scale: textScaleFactor,
                baseline: currentDimensions.baseline,
                baselineOffset: currentDimensions.baselineOffset
            );
            if (hasStyle) {
                builder.pop();
            }
        }

        public override bool visitChildren(InlineSpanVisitor visitor) {
            return visitor(this);
        }


        protected override InlineSpan getSpanForPositionVisitor(TextPosition position, Accumulator offset) {
            if (position.offset == offset.value) {
                return this;
            }

            offset.increment(1);
            return null;
        }

        protected override int? codeUnitAtVisitor(int index, Accumulator offset) {
            return null;
        }

        public override RenderComparison compareTo(InlineSpan other) {
            if (this == other)
                return RenderComparison.identical;
            if ((this.style == null) != (other.style == null))
                return RenderComparison.layout;
            WidgetSpan typedOther = other as WidgetSpan;
            if (this.child.Equals(typedOther.child) || this.alignment != typedOther.alignment) {
                return RenderComparison.layout;
            }

            RenderComparison result = RenderComparison.identical;
            if (this.style != null) {
                RenderComparison candidate = this.style.compareTo(other.style);
                if ((int) candidate > (int) result)
                    result = candidate;
                if (result == RenderComparison.layout)
                    return result;
            }

            return result;
        }

        public static bool operator ==(WidgetSpan current, object other) {
            if (ReferenceEquals(current, other))
                return true;
            if (current == null) {
                return false;
            }
            return other is WidgetSpan otherSpan
                   && otherSpan.child.Equals(current.child)
                   && otherSpan.alignment == current.alignment
                   && otherSpan.baseline == current.baseline;
        }

        public static bool operator !=(WidgetSpan current, object other) {
            return !(current == other);
        }

        public override int GetHashCode() {
            int hashCode = base.GetHashCode();
            hashCode = (hashCode * 397) ^ (this.child.GetHashCode());
            hashCode = (hashCode * 397) ^ (this.alignment.GetHashCode());
            hashCode = (hashCode * 397) ^ (this.baseline.GetHashCode());
            return hashCode;
        }

        public bool debugAssertIsValid() {
            return true;
        }
    }
}