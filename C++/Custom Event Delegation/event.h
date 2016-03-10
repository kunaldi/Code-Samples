//=============================================================================================
//					Copyright 2004 Sploosh, All rights reserved.
//
// File: Event class, base structure of event data
//
//=============================================================================================

#ifndef __CEVENT_H__
#define __CEVENT_H__

#include "event_operators.h"
#include "delegate.h"

__BEG_SPK_NAMESPACE

//---------------------------------------------------------------------------------------------
//
// EVENT INTERFACE
//
//---------------------------------------------------------------------------------------------
template<class R, class CT, class AT> // return class(arg)
class DLL_API IEvent : public IDelegate<AT>
//---------------------------------------------------------------------------------------------
{
public:	// Methods

	IEvent() : errCode(0)
	{
		//time_raisedAt = cTime::getNow();
		//gIOMng.print("Event raised at: " + time_raisedAt.toString(), ltDefault);
			//dwStart = GetTickCount();
	}

	IEvent(eEventTypes _et, cStrA _str, int _err)
	: eventType(_et), strData(_str), errCode(_err)
	{
		//time_raisedAt = cTime::getNow();
		//gIOMng.print("Event raised at: " + time_raisedAt.toString(), ltDefault);
			//dwStart = GetTickCount();
	}

	inline cDelegate<R, CT, AT>* getDelegate()
	{
		return &eventDelegate;
	}

	inline void invoke (AT* _pArg)
	{
		//time_processedAt = cTime::getNow();	
		eventDelegate.invoke(_pArg);
			//dwStop = GetTickCount();
			//dwDiff = (dwStop - dwStart);
			//gIOMng.print("Event processed in milliseconds: " + itos(dwDiff) /*time_processedAt.toString()*/, ltDefault);
	}

	inline void operator ()(AT* _pArg)
	{
		//time_processedAt = cTime::getNow();	
		eventDelegate.invoke(_pArg);
			//dwStop = GetTickCount();
			//dwDiff = (dwStop - dwStart);
			//gIOMng.print("Event processed in milliseconds: " + itos(dwDiff) /*time_processedAt.toString()*/, ltDefault);
	}

public:	// Attributes

	eEventTypes eventType;
	String strData;
	int errCode; // TODO: Should be enum instead of int

//	cTime time_raisedAt, time_processedAt;

	DWORD dwStart, dwStop, dwDiff;

protected:
	cDelegate<R, CT, AT> eventDelegate;

private:
};
//---------------------------------------------------------------------------------------------
//
// SOCKET EVENT CLASS
//
//---------------------------------------------------------------------------------------------
class DLL_API cSocketEvent : public IEvent<void, cSocketEventsOperator, cSocketEvent>
//---------------------------------------------------------------------------------------------
{
public:
	// EventType, strData, errCode, dataLength
	cSocketEvent(eEventTypes _et, cStrA _str, int _err, int _len)
	: IEvent<void, cSocketEventsOperator, cSocketEvent>
	(_et, _str, _err), dataLength(_len) {}

	cSocketEvent() {}
	~cSocketEvent() {}

	// Event Attributes
	// ----------------
	eSocketEvents socketEvent;
	int dataLength;
};
//---------------------------------------------------------------------------------------------
//
// GUI EVENT CLASS
//
//---------------------------------------------------------------------------------------------
class DLL_API cGUIEvent : public IEvent<void, cGUIEventsOperator, cGUIEvent>
//---------------------------------------------------------------------------------------------
{
public:
	// EventType, strData, errCode, x, y, keys, delta
	cGUIEvent(eEventTypes _et, cStrA _str, int _err, int _x, int _y, int _keys, int _delta)
	: IEvent<void, cGUIEventsOperator, cGUIEvent>
	(_et, _str, _err), x(_x), y(_y), keys(_keys), delta(_delta) {}

	cGUIEvent() {}
	~cGUIEvent() {}

	// Event Attributes
	// ----------------
	eGUIEvents GUIEvent;
	Gui_Base::IGUI_Object* pObject;
	int x, y, keys, delta;
};
//---------------------------------------------------------------------------------------------
//
// CMD EVENT CLASS
//
//---------------------------------------------------------------------------------------------
class DLL_API cCMDEvent : public IEvent<void, cCMDEventsOperator, cCMDEvent>
//---------------------------------------------------------------------------------------------
{
public:
	// EventType, strData, errCode, x, y, keys, delta
	cCMDEvent(eEventTypes _et, cStrA _str, int _err)
	: IEvent<void, cCMDEventsOperator, cCMDEvent>
	(_et, _str, _err) {}

	cCMDEvent() {}

	// Event Attributes
	// ----------------
	eCMDEvents CMDEvent;
	cPacket::sPacketBody CMDArgs;
};




__END_SPK_NAMESPACE

#endif
