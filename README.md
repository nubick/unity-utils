# unity-utils
Different help scripts for Unity engine.

Random Util
======
Unity provides simple Random.Range(min, max) method for int and float numeric types. Real projects need more complex random behavior and number of different advanced methods. Methods from [RandomUtil.cs](//github.com/nubick/unity-utils/blob/master/sources/Assets/Scripts/Utils/RandomUtil.cs) are based on Random.Range and make live a little easier.

    //Return random bool value.
    public static bool NextBool()
    Ex:bool randomBool = RandomUtil.NextBool();
    
    //Return random item from item1, item2 or item3 items set.
    public static T Next<T>(T item1, T item2)
    public static T Next<T>(T item1, T item2, T item3)
    Ex:string randomPerson = RandomUtil.Next("me", "you");
    Ex:Pork randomPork =RandomUtil.Next(Pork.NifNif, Pork.NafNaf, Pork.NufNuf);
    
    //Return random item from array.
    public static T NextItem<T>(T[] array)
    Ex:int[] intArray = new[] { 1, 3, 5, 7, 9 };
    Ex:int randomInt = RandomUtil.NextItem(intArray);
    
    //Return random item from list.
    public static T NextItem<T>(List<T> list)
    Ex:List<int> list = new List<int> { 1, 3, 5, 7, 9 };
    Ex:int randomInt = RandomUtil.NextItem(list);
    
    //Return random enum item.
   	public static T NextEnum<T>()
    Ex:enum Direction { Left, Right, Up, Down }
    Ex:Direction randomDirection = RandomUtil.NextEnum<Direction>();
    
    //Return random index of passed array. Index random selection is based on array weights.
    public static int NextWeightedInd(int[] weights)
    public static int NextWeightedInd(float[] weights)
    Ex:int[] weights = new int[] { 10, 10, 30, 50 };
    Ex:int randomIndex = RandomUtil.NextWeightedInd(weights);
    
    //Return sub-list of random items from origin list without repeating.
    public static List<T> Take<T>(List<T> list, int count)
    Ex:List<Card> deckCards = GetDeck();
    Ex:List<Card> handCards = RandomUtil.Take(deckCards, 6);
    
    //Shuffle list of items.
    public static void Shuffle<T>(this List<T> list)
    Ex:List<int> list = new List<int> { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
    Ex:RandomUtil.Shuffle(list);
    
    //Shuffle array of items.
    public static void Shuffle<T>(T[] array)
    Ex:float[] array = new float[] { 1.1f, 1.2f, 1.3f, 1.5f, 1.6f };
    RandomUtil.Shuffle(array);


Utils Extensions
======
Different extensions sugar-methods for often used operations.

[UtilsExtensions.cs](//github.com/nubick/unity-utils/blob/master/sources/Assets/Scripts/Utils/UtilsExtensions.cs).

Invoke wrappers
------
Type-safe versions of Invoke functions from MonoBehaviour.

Instead using 

    1.  Invoke("InvokedFunc", 1f);
    2.  InvokeRepeating("InvokedFunc", 1f, 1f);
    3.  bool isInvoking = IsInvoking("InvokedFunc");
    4.  CancelInvoke("InvokedFunc");

we can use type-safe methods:

    1.  this.Invoke(() => InvokedFunc(), 1f);
    2.  this.InvokeRepeating(() => InvokedFunc(), 1f, 1f);
    3.  bool isInvoking = this.IsInvoking(() => InvokedFunc());
    4.  this.CancelInvoke(() => InvokedFunc());

    

Visual Studio Snippets
======
Install guide
------

1. Download snippets from
[releases](//github.com/nubick/unity-utils/releases).

2. Open Visual Studio Menu -> Tools -> Code Snippets Manager.

3. Add folder with snippets.

Snippets
------
1.Write 'dlog' in VS editor and press 'tab' button. You will get:

    Debug.Log("");

Also, you can use 'dlog1', 'dlog2', 'dlog3', 'dlog4' and 'dlog5' words for Debug.Log with parameters. Check following example:

![My image](http://nubick.github.com/readme/dlog-snippets.gif)


2.Write 'glh' in VS editor and press 'tab' button. You will get:

    GUILayout.BeginHorizontal();
    
    GUILayout.EndHorizontal();

3.Write 'glv' in VS editor and press 'tab' button. You will get:

    GUILayout.BeginVertical();
    
    GUILayout.EndVertical();
