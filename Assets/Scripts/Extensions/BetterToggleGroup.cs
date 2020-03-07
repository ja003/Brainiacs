using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class BetterToggleGroup : ToggleGroup
{
	public delegate void ChangedEventHandler(Toggle newActive);

	public event ChangedEventHandler OnChange;
	
	protected override void Start()
	{
		//base.Start();
		foreach(Transform transformToggle in gameObject.transform)
		{
			var toggle = transformToggle.gameObject.GetComponent<Toggle>();
			toggle.onValueChanged.AddListener((isSelected) => {
				if(!isSelected)
				{
					return;
				}
				var activeToggle = Active();
				DoOnChange(activeToggle);
			});
		}
	}
	public Toggle Active()
	{
		return ActiveToggles().FirstOrDefault();
	}

	protected virtual void DoOnChange(Toggle newactive)
	{
		var handler = OnChange;
		if(handler != null) handler(newactive);
	}
}
