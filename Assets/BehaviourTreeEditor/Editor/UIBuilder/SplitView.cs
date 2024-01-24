using UnityEngine.UIElements;

namespace Editor
{
    public class SplitView : TwoPaneSplitView
    {
        public new class UxmlFactory : UxmlFactory<SplitView, UxmlTraits>
        {
        }

        public new float fixedPaneInitialDimension
        {
            get => base.fixedPaneInitialDimension;
            // set => base.fixedPaneInitialDimension = value;
        }
    }
}