using System;
using System.Collections.Generic;
using System.Text;
using Unity.UIWidgets.foundation;
using Unity.UIWidgets.gestures;
using Unity.UIWidgets.ui;

namespace Unity.UIWidgets.painting {
    public class Accumulator {
        public Accumulator(int value = 0) {
            this._value = value;
        }

        public int value {
            get { return this._value; }
        }

        int _value;

        public void increment(int addend) {
            D.assert(addend >= 0);
            this._value += addend;
        }
    }

    public delegate bool InlineSpanVisitor(InlineSpan span);

    public class InlineSpanSemanticsInformation : IEquatable<InlineSpanSemanticsInformation> {
        public InlineSpanSemanticsInformation(
            string text,
            GestureRecognizer recognizer = null,
            string semanticsLabel = "",
            bool isPlaceholder = false
        ) {
            D.assert(text != null);
            D.assert(isPlaceholder != null);
            D.assert(isPlaceholder == false || (text == "\uFFFC" && semanticsLabel == null && recognizer == null));
            this.text = text;
            this.semanticsLabel = semanticsLabel;
            this.recognizer = recognizer;
            this.isPlaceholder = isPlaceholder;
            this.requiresOwnNode = isPlaceholder || recognizer != null;
        }

        public static InlineSpanSemanticsInformation placeholder =
            new InlineSpanSemanticsInformation("\uFFFC", isPlaceholder: true);

        string text;

        string semanticsLabel;

        GestureRecognizer recognizer;

        bool isPlaceholder;

        bool requiresOwnNode;

        public static bool operator ==(InlineSpanSemanticsInformation current, Object other) {
            return current.Equals(other);
        }

        public static bool operator !=(InlineSpanSemanticsInformation current, object other) {
            return !(current == other);
        }

        public override string ToString() {
            return
                $" InlineSpanSemanticsInformation text: {this.text}, semanticsLabel: {this.semanticsLabel}, recognizer: {this.recognizer}";
        }


        public override int GetHashCode() {
            unchecked {
                var hashCode = (this.text != null ? this.text.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (this.semanticsLabel != null ? this.semanticsLabel.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (this.recognizer != null ? this.recognizer.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ this.isPlaceholder.GetHashCode();
                hashCode = (hashCode * 397) ^ this.requiresOwnNode.GetHashCode();
                return hashCode;
            }
        }

        public bool Equals(InlineSpanSemanticsInformation other) {
            if (ReferenceEquals(null, other)) {
                return false;
            }

            if (ReferenceEquals(this, other)) {
                return true;
            }

            return this.text == other.text && this.semanticsLabel == other.semanticsLabel &&
                   Equals(this.recognizer, other.recognizer) && this.isPlaceholder == other.isPlaceholder &&
                   this.requiresOwnNode == other.requiresOwnNode;
        }

        public override bool Equals(object obj) {
            if (ReferenceEquals(null, obj)) {
                return false;
            }

            if (ReferenceEquals(this, obj)) {
                return true;
            }

            if (obj.GetType() != this.GetType()) {
                return false;
            }

            return this.Equals((InlineSpanSemanticsInformation) obj);
        }
    }


    public abstract class InlineSpan : DiagnosticableTree {
        public InlineSpan(TextStyle style, HoverRecognizer hoverRecognizer) {
            this.style = style;
            this.hoverRecognizer = hoverRecognizer;
        }

        public readonly HoverRecognizer hoverRecognizer;

        public bool hasHoverRecognizer {
            get {
                bool need = false;
                this.visitChildren((text) => {
                    if (text.hoverRecognizer != null) {
                        need = true;
                        return false;
                    }

                    return true;
                });
                return need;
            }
        }

        public TextStyle style;

        public abstract void build(ui.ParagraphBuilder builder, List<PlaceholderDimensions> dimensions = null,
            float textScaleFactor = 1.0f);

        public abstract bool visitChildren(InlineSpanVisitor visitor);

        public InlineSpan getSpanForPosition(TextPosition position) {
            D.assert(this.debugAssertIsValid());
            Accumulator offset = new Accumulator();
            InlineSpan result = null;
            this.visitChildren((InlineSpan span) => {
                result = span.getSpanForPositionVisitor(position, offset);
                return result == null;
            });
            return result;
        }

        protected abstract InlineSpan getSpanForPositionVisitor(TextPosition position, Accumulator offset);

        public string toPlainText(
            bool includeSemanticsLabels = true, bool includePlaceholders = true
        ) {
            StringBuilder buffer = new StringBuilder();
            this.computeToPlainText(buffer, includeSemanticsLabels: includeSemanticsLabels,
                includePlaceholders: includePlaceholders);
            return buffer.ToString();
        }

        List<InlineSpanSemanticsInformation> getSemanticsInformation() {
            List<InlineSpanSemanticsInformation> collector = new List<InlineSpanSemanticsInformation>();
            this.computeSemanticsInformation(collector);
            return collector;
        }

        protected internal abstract void computeSemanticsInformation(List<InlineSpanSemanticsInformation> collector);

        protected internal abstract void computeToPlainText(StringBuilder buffer,
            bool includeSemanticsLabels = true, bool includePlaceholders = true
        );

        public int? codeUnitAt(int index) {
            if (index < 0)
                return null;
            Accumulator offset = new Accumulator();
            int? result = null;
            this.visitChildren((InlineSpan span) => {
                result = span.codeUnitAtVisitor(index, offset);
                return result == null;
            });
            return result;
        }

        protected abstract int? codeUnitAtVisitor(int index, Accumulator offset);

        public bool debugAssertIsValid() => true;

        public abstract RenderComparison compareTo(InlineSpan other);

        public static bool operator ==(InlineSpan current, Object other) {
            return Equals(current, other);
        }

        public static bool operator !=(InlineSpan current, object other) {
            return !(current == other);
        }

        public override bool Equals(object other) {
            if (ReferenceEquals(other, null)) {
                return false;
            }
            if (ReferenceEquals(this, other)) {
                return true;
            }

            if (other is InlineSpan otherSpan) {
                return otherSpan.style == this.style;
            }
            return false;
        }

        public override void debugFillProperties(DiagnosticPropertiesBuilder properties) {
            base.debugFillProperties(properties);
            properties.defaultDiagnosticsTreeStyle = DiagnosticsTreeStyle.whitespace;

            if (this.style != null) {
                this.style.debugFillProperties(properties);
            }
        }
    }
}