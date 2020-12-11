/*
//=========================================//
AUTHOR:		Karel Brezina
DATE:		2018-08-21
FUNCTION:	Specify type of singleton object
//=========================================//
*/

using System;


public class CSingletionAttribute : Attribute
{
	//=========================================//
	// GET & SET
	//=========================================//

	public string Name { get; }
	public bool IsPersistent { get; }

	//=========================================//
	// PUBLIC METHODS
	//=========================================//

	public CSingletionAttribute(bool pPersistant)
	{
		this.IsPersistent = pPersistant;
	}

	public CSingletionAttribute(string pName, bool pPersistant)
	{
		this.Name = pName;
		this.IsPersistent = pPersistant;
	}
}
