using System;
using System.Collections.Generic;
using System.Linq;
using Unity.UIWidgets.ui;
using Unity.UIWidgets.foundation;
using Unity.UIWidgets.widgets;
using Unity.UIWidgets.painting;
namespace Unity.UIWidgets.cupertino {
    public class DiagnosticableMixinColor : Color, IDiagnosticable {
        protected DiagnosticableMixinColor(uint value) :base(value){
        }

        public virtual string toStringShort() {
            return foundation_.describeIdentity(this);
        }

        public override string ToString() {
            return toString();
        }

        public virtual string toString(DiagnosticLevel minLevel = DiagnosticLevel.debug) {
            string fullString = null;
            D.assert(() => {
                fullString = toDiagnosticsNode(style: DiagnosticsTreeStyle.singleLine)
                    .toString(minLevel: minLevel);
                return true;
            });
            return fullString ?? toStringShort();
        }

        public virtual DiagnosticsNode toDiagnosticsNode(
            string name = null,
            DiagnosticsTreeStyle style = DiagnosticsTreeStyle.sparse) {
            return new DiagnosticableNode<DiagnosticableMixinColor>(
                name: name, value: this, style: style
            );
        }

        public virtual void debugFillProperties(DiagnosticPropertiesBuilder properties) {
        }
    }

}

