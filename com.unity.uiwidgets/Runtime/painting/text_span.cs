using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.UIWidgets.foundation;
using Unity.UIWidgets.gestures;
using Unity.UIWidgets.ui;
using UnityEngine.Assertions;

namespace Unity.UIWidgets.painting {
    public class TextSpan : InlineSpan, IEquatable<TextSpan> {
        public delegate bool Visitor(TextSpan span);

        public readonly TextStyle style;
        public readonly string text;
        public List<string> splitedText;
        public readonly List<InlineSpan> children;
        public readonly GestureRecognizer recognizer;
        public readonly String semanticsLabel;

        public TextSpan(string text = "", TextStyle style = null, List<InlineSpan> children = null,
            GestureRecognizer recognizer = null, HoverRecognizer hoverRecognizer = null) : base(style: style,
            hoverRecognizer: hoverRecognizer) {
            this.text = text;
            this.splitedText = !string.IsNullOrEmpty(text) ? EmojiUtils.splitByEmoji(text) : null;
            this.style = style;
            this.children = children;
            this.recognizer = recognizer;
        }

        public override void build(ui.ParagraphBuilder builder, List<PlaceholderDimensions> dimensions,
            float textScaleFactor = 1.0f) {
            var hasStyle = this.style != null;

            if (hasStyle) {
                builder.pushStyle(this.style, textScaleFactor);
            }

            if (this.splitedText != null) {
                if (this.splitedText.Count == 1 && !char.IsHighSurrogate(this.splitedText[0][0]) &&
                    !EmojiUtils.isSingleCharEmoji(this.splitedText[0][0])) {
                    builder.addText(this.splitedText[0]);
                }
                else {
                    TextStyle style = this.style ?? new TextStyle();
                    for (int i = 0; i < this.splitedText.Count; i++) {
                        builder.pushStyle(style, textScaleFactor);
                        builder.addText(this.splitedText[i]);
                        builder.pop();
                    }
                }
            }


            if (this.children != null) {
                foreach (var child in this.children) {
                    Assert.IsNotNull(child);
                    child.build(builder, dimensions, textScaleFactor);
                }
            }

            if (hasStyle) {
                builder.pop();
            }
        }

        public override bool visitChildren(InlineSpanVisitor visitor) {
            if (this.text != null) {
                if (!visitor.Invoke(this))
                    return false;
            }

            if (this.children != null) {
                foreach (InlineSpan child in this.children) {
                    if (!child.visitChildren(visitor)) {
                        return false;
                    }
                }
            }

            return true;
        }


        protected override InlineSpan getSpanForPositionVisitor(TextPosition position, Accumulator offset) {
            D.assert(this.debugAssertIsValid());
            var targetOffset = position.offset;
            var affinity = position.affinity;
            int endOffset = offset.value + this.text.Length;
            if (offset.value == targetOffset && affinity == TextAffinity.downstream ||
                offset.value < targetOffset && targetOffset < endOffset ||
                endOffset == targetOffset && affinity == TextAffinity.upstream) {
                return this;
            }

            offset.increment(this.text.Length);
            return null;
        }

        protected internal override void computeToPlainText(
            StringBuilder buffer,
            bool includeSemanticsLabels = true,
            bool includePlaceholders = true) {
            D.assert(this.debugAssertIsValid());
            if (this.semanticsLabel != null && includeSemanticsLabels) {
                buffer.Append(this.semanticsLabel);
            }
            else if (this.text != null) {
                buffer.Append(this.text);
            }

            if (this.children != null) {
                foreach (InlineSpan child in this.children) {
                    child.computeToPlainText(buffer,
                        includeSemanticsLabels: includeSemanticsLabels,
                        includePlaceholders: includePlaceholders
                    );
                }
            }
        }

        protected internal override void computeSemanticsInformation(List<InlineSpanSemanticsInformation> collector) {
            D.assert(this.debugAssertIsValid());
            if (this.text != null || this.semanticsLabel != null) {
                collector.Add(new InlineSpanSemanticsInformation(
                    this.text,
                    semanticsLabel: this.semanticsLabel,
                    recognizer: this.recognizer
                ));
            }

            if (this.children != null) {
                foreach (InlineSpan child in this.children) {
                    child.computeSemanticsInformation(collector);
                }
            }
        }

        protected override int? codeUnitAtVisitor(int index, Accumulator offset) {
            if (this.text == null) {
                return null;
            }

            if (index - offset.value < this.text.Length) {
                return this.text[index - offset.value];
            }

            offset.increment(this.text.Length);
            return null;
        }

        bool debugAssertIsValid() {
            D.assert(() => {
                foreach (InlineSpan child in this.children) {
                    if (child == null) {
                        throw new UIWidgetsError(
                            "A TextSpan object with a non-null child list should not have any nulls in its child list.\n" +
                            "The full text in question was:\n" +
                            this.toStringDeep(prefixLineOne: "  "));
                    }

                    D.assert(child.debugAssertIsValid());
                }

                return true;
            });
            return base.debugAssertIsValid();
        }

        public override RenderComparison compareTo(InlineSpan otherInline) {
            if (this.Equals(otherInline)) {
                return RenderComparison.identical;
            }

            TextSpan other = otherInline as TextSpan;
            if (other.text != this.text
                || ((this.children == null) != (other.children == null))
                || (this.children != null && other.children != null && this.children.Count != other.children.Count)
                || ((this.style == null) != (other.style != null))
            ) {
                return RenderComparison.layout;
            }

            RenderComparison result = Equals(this.recognizer, other.recognizer)
                ? RenderComparison.identical
                : RenderComparison.metadata;

            if (!Equals(this.hoverRecognizer, other.hoverRecognizer)) {
                result = RenderComparison.function > result ? RenderComparison.function : result;
            }

            if (this.style != null) {
                var candidate = this.style.compareTo(other.style);
                if (candidate > result) {
                    result = candidate;
                }

                if (result == RenderComparison.layout) {
                    return result;
                }
            }

            if (this.children != null) {
                for (var index = 0; index < this.children.Count; index++) {
                    var candidate = this.children[index].compareTo(other.children[index]);
                    if (candidate > result) {
                        result = candidate;
                    }

                    if (result == RenderComparison.layout) {
                        return result;
                    }
                }
            }

            return result;
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

            if (obj is TextSpan other) {
                return this.Equals(other);
            }
            else {
                return false;
            }
        }

        public override int GetHashCode() {
            unchecked {
                var hashCode = (this.style != null ? this.style.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (this.text != null ? this.text.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (this.splitedText != null ? this.splitedText.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (this.children != null ? this.children.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (this.recognizer != null ? this.recognizer.GetHashCode() : 0);
                hashCode = (hashCode * 397) ^ (this.semanticsLabel != null ? this.semanticsLabel.GetHashCode() : 0);
                return hashCode;
            }
        }

        public bool Equals(TextSpan other) {
            if (ReferenceEquals(null, other)) {
                return false;
            }

            if (ReferenceEquals(this, other)) {
                return true;
            }

            return Equals(this.style, other.style) && string.Equals(this.text, other.text) &&
                   childEquals(this.children, other.children) && this.recognizer == other.recognizer;
        }

        public static bool operator ==(TextSpan left, TextSpan right) {
            return Equals(left, right);
        }

        public static bool operator !=(TextSpan left, TextSpan right) {
            return !Equals(left, right);
        }

        int childHash() {
            unchecked {
                var hashCode = 0;
                if (this.children != null) {
                    foreach (var child in this.children) {
                        hashCode = (hashCode * 397) ^ (child != null ? child.GetHashCode() : 0);
                    }
                }

                return hashCode;
            }
        }

        static bool childEquals(List<InlineSpan> left, List<InlineSpan> right) {
            if (ReferenceEquals(left, right)) {
                return true;
            }

            if (left == null || right == null) {
                return false;
            }

            return left.SequenceEqual(right);
        }

        public override void debugFillProperties(DiagnosticPropertiesBuilder properties) {
            base.debugFillProperties(properties);
            properties.defaultDiagnosticsTreeStyle = DiagnosticsTreeStyle.whitespace;
            // Properties on style are added as if they were properties directly on
            // this TextSpan.
            if (this.style != null) {
                this.style.debugFillProperties(properties);
            }

            properties.add(new DiagnosticsProperty<GestureRecognizer>(
                "recognizer", this.recognizer,
                description: this.recognizer == null ? "" : this.recognizer.GetType().FullName,
                defaultValue: Diagnostics.kNullDefaultValue
            ));

            properties.add(new StringProperty("text", this.text, showName: false,
                defaultValue: Diagnostics.kNullDefaultValue));
            if (this.style == null && this.text == null && this.children == null) {
                properties.add(DiagnosticsNode.message("(empty)"));
            }
        }

        public override List<DiagnosticsNode> debugDescribeChildren() {
            if (this.children == null) {
                return new List<DiagnosticsNode>();
            }

            return this.children.Select((child) => {
                if (child != null) {
                    return child.toDiagnosticsNode();
                }
                else {
                    return DiagnosticsNode.message("<null child>");
                }
            }).ToList();
        }
    }
}