using System.Collections.Generic;
using System.Linq;
using System.Text;
using Unity.UIWidgets.external;

namespace Unity.UIWidgets.foundation {
    public class DiagnosticableTreeMixinChangeNotifier : ChangeNotifier, DiagnosticableTreeMixin {
        protected DiagnosticableTreeMixinChangeNotifier() {
        }
        public virtual string toString( DiagnosticLevel minLevel = DiagnosticLevel.info ) {
            return toDiagnosticsNode(style: DiagnosticsTreeStyle.singleLine).toString(minLevel: minLevel);
        }

        public virtual string toStringShallow(
            string joiner = ", ",
            DiagnosticLevel minLevel = DiagnosticLevel.debug
        ) {
            if (foundation_.kReleaseMode) {
                return toString();
            }
            string shallowString = "";
            D.assert(() => {
                var result = new StringBuilder();
                result.Append(toString());
                result.Append(joiner);
                DiagnosticPropertiesBuilder builder = new DiagnosticPropertiesBuilder();
                debugFillProperties(builder);
                var property = ExternalUtils<DiagnosticsNode>.WhereList(builder.properties, (n => !n.isFiltered(minLevel)));
                result.Append(string.Join(joiner,
                    ExternalUtils<string,DiagnosticsNode>.SelectList(property,(n => n.ToString()))
                    )
                );
                shallowString = result.ToString();
                return true;
            });
            return shallowString;
        }

        public virtual string toStringDeep(
            string prefixLineOne = "",
            string prefixOtherLines = null,
            DiagnosticLevel minLevel = DiagnosticLevel.debug
        ) {
            return toDiagnosticsNode().toStringDeep(prefixLineOne: prefixLineOne, prefixOtherLines: prefixOtherLines, minLevel: minLevel);
        }

        public virtual string toStringShort() {
            return foundation_.describeIdentity(this);
        }

        public virtual DiagnosticsNode toDiagnosticsNode(
            string name = null, 
            DiagnosticsTreeStyle style = DiagnosticsTreeStyle.sparse) {
            return new DiagnosticableTreeNode(
                name: name,
                value: this,
                style: style
            );
        }

        public virtual List<DiagnosticsNode> debugDescribeChildren() 
        {
            return new List<DiagnosticsNode>();
        }

        public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties) { }
    }

}

