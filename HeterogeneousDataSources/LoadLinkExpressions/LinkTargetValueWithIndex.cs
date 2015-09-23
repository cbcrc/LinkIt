namespace HeterogeneousDataSources.LoadLinkExpressions
{
    public class LinkTargetValueWithIndex<TTargetValue> {
        public LinkTargetValueWithIndex(TTargetValue targetValue, int index){
            TargetValue = targetValue;
            Index = index;
        }

        public TTargetValue TargetValue { get; private set; }
        public int Index { get; private set; }
    }
}