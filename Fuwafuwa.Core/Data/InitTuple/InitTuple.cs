namespace Fuwafuwa.Core.Data.InitTuple;

public class InitTuple<TItem> where TItem : new() {
    public InitTuple() {
        Item1 = new TItem();
    }

    public InitTuple(TItem item1) {
        Item1 = item1;
    }

    public TItem Item1 { get; set; }
}

public class InitTuple<TItem1, TItem2> where TItem1 : new() where TItem2 : new() {
    public InitTuple() {
        Item1 = new TItem1();
        Item2 = new TItem2();
    }

    public InitTuple(TItem1 item1, TItem2 item2) {
        Item1 = item1;
        Item2 = item2;
    }

    public TItem1 Item1 { get; set; }
    public TItem2 Item2 { get; set; }
}