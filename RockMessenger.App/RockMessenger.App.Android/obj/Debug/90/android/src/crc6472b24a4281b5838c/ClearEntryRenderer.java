package crc6472b24a4281b5838c;


public class ClearEntryRenderer
	extends crc643f46942d9dd1fff9.EntryRenderer
	implements
		mono.android.IGCUserPeer
{
/** @hide */
	public static final String __md_methods;
	static {
		__md_methods = 
			"";
		mono.android.Runtime.register ("RockMessenger.App.Droid.Components.ClearEntryRenderer, RockMessenger.App.Android", ClearEntryRenderer.class, __md_methods);
	}


	public ClearEntryRenderer (android.content.Context p0, android.util.AttributeSet p1, int p2)
	{
		super (p0, p1, p2);
		if (getClass () == ClearEntryRenderer.class)
			mono.android.TypeManager.Activate ("RockMessenger.App.Droid.Components.ClearEntryRenderer, RockMessenger.App.Android", "Android.Content.Context, Mono.Android:Android.Util.IAttributeSet, Mono.Android:System.Int32, mscorlib", this, new java.lang.Object[] { p0, p1, p2 });
	}


	public ClearEntryRenderer (android.content.Context p0, android.util.AttributeSet p1)
	{
		super (p0, p1);
		if (getClass () == ClearEntryRenderer.class)
			mono.android.TypeManager.Activate ("RockMessenger.App.Droid.Components.ClearEntryRenderer, RockMessenger.App.Android", "Android.Content.Context, Mono.Android:Android.Util.IAttributeSet, Mono.Android", this, new java.lang.Object[] { p0, p1 });
	}


	public ClearEntryRenderer (android.content.Context p0)
	{
		super (p0);
		if (getClass () == ClearEntryRenderer.class)
			mono.android.TypeManager.Activate ("RockMessenger.App.Droid.Components.ClearEntryRenderer, RockMessenger.App.Android", "Android.Content.Context, Mono.Android", this, new java.lang.Object[] { p0 });
	}

	private java.util.ArrayList refList;
	public void monodroidAddReference (java.lang.Object obj)
	{
		if (refList == null)
			refList = new java.util.ArrayList ();
		refList.add (obj);
	}

	public void monodroidClearReferences ()
	{
		if (refList != null)
			refList.clear ();
	}
}
