//=============================================================================================
//					Copyright 2004 Sploosh, All rights reserved.
//
// File: EventListeners class, implementations of event types
//
//=============================================================================================

#include "spellkernel.h"

#include "socket_manager.h"
#include "process_manager.h"

#include "event_operators.h"

__BEG_SPK_NAMESPACE

//---------------------------------------------------------------------------------------------
//
// SOCKET EVENTS OPERATOR
//
//---------------------------------------------------------------------------------------------
cSocketEventsOperator::cSocketEventsOperator(cText _processName)
//---------------------------------------------------------------------------------------------
{
	//pEventProcessor = reinterpret_cast<socketEventProc*>(gProcessMng.getProcess(_processName));
	gProcessMng.getProcess(pEventProcessor, _processName);
}
// Generate and push an appropriate event to EventProcessor for SOCKET operations
//---------------------------------------------------------------------------------------------
void cSocketEventsOperator::raiseEvent(eSocketEvents sockEvent, cText szVal)
//---------------------------------------------------------------------------------------------
{
	typedef cSocketEventsOperator _callback;

	pSockEvt pEvent = new cSocketEvent();

	pEvent->eventType = EVENTTYPE_SOCKET;
	pEvent->socketEvent = sockEvent;
	pEvent->strData = szVal;

	// Detailed event customization
	switch(sockEvent)
	{
	case EVENT_SOCK_ONRECEIVE:
		pEvent->getDelegate()->addRef(this, _callback::onReceive);
		break;

	case EVENT_SOCK_ONSEND:
		pEvent->getDelegate()->addRef(this, _callback::onSend);
		break;

	case EVENT_SOCK_ONLISTEN:
		pEvent->getDelegate()->addRef(this, _callback::onListen);
		break;

	case EVENT_SOCK_ONACCEPT:
		pEvent->getDelegate()->addRef(this, _callback::onAccept);
		break;

	case EVENT_SOCK_ONCONNECT:
		pEvent->getDelegate()->addRef(this, _callback::onConnect);
		break;

	case EVENT_SOCK_ONCREATE:
		pEvent->getDelegate()->addRef(this, _callback::onCreate);
		break;

	case EVENT_SOCK_ONDISCONNECT:
		pEvent->getDelegate()->addRef(this, _callback::onDisconnect);
		break;

	case EVENT_SOCK_ONCLOSE:
		pEvent->getDelegate()->addRef(this, _callback::onClose);
		break;

	case EVENT_SOCK_ONERROR:
		pEvent->getDelegate()->addRef(this, _callback::onError);
		break;
	}

	pEventProcessor->addItem(pEvent);
}
//---------------------------------------------------------------------------------------------
void cSocketEventsOperator::raiseEvent(eSocketEvents sockEvent, int iVal)
//---------------------------------------------------------------------------------------------
{
	typedef cSocketEventsOperator _callback;

	pSockEvt pEvent = new cSocketEvent();

	pEvent->eventType = EVENTTYPE_SOCKET;
	pEvent->socketEvent = sockEvent;
	pEvent->errCode = iVal;

	// Detailed event customization
	switch(sockEvent)
	{
	case EVENT_SOCK_ONERROR:
		pEvent->getDelegate()->addRef(this, _callback::onError);
		break;
	}

	pEventProcessor->addItem(pEvent);
}
//---------------------------------------------------------------------------------------------
//
// GUI EVENTS OPERATOR
//
//---------------------------------------------------------------------------------------------
cGUIEventsOperator::cGUIEventsOperator(cText _processName)
//---------------------------------------------------------------------------------------------
{
	gProcessMng.getProcess(pEventProcessor, _processName);
}
// Generate and push an appropriate event to EventProcessor for GUI operations
//---------------------------------------------------------------------------------------------
void cGUIEventsOperator::raiseEvent(eGUIEvents GUIEvent, Gui_Base::IGUI_Object* pObject, int x, int y, int keys)
//---------------------------------------------------------------------------------------------
{
	typedef cGUIEventsOperator _callback;

	pGUIEvt pEvent = new cGUIEvent();

	pEvent->eventType = EVENTTYPE_GUI;
	pEvent->GUIEvent = GUIEvent;
	pEvent->pObject = pObject;
	pEvent->keys = keys;
	pEvent->x = x;
	pEvent->y = x;

	// Detailed event customization
	switch(GUIEvent)
	{
	case EVENT_GUI_ONLEFTBUTTONUP:
		pEvent->getDelegate()->addRef(this, _callback::onLeftButtonUp);
		break;

	case EVENT_GUI_ONLEFTBUTTONDOWN:
		pEvent->getDelegate()->addRef(this, _callback::onLeftButtonDown);
		break;

	}

	pEventProcessor->addItem(pEvent);
}
//---------------------------------------------------------------------------------------------
//
// CMD EVENTS OPERATOR
//
//---------------------------------------------------------------------------------------------
cCMDEventsOperator::cCMDEventsOperator(cText _processName)
//---------------------------------------------------------------------------------------------
{
	gProcessMng.getProcess(pEventProcessor, _processName);
}
// Generate and push an appropriate event to EventProcessor for CMD operations
//---------------------------------------------------------------------------------------------
void cCMDEventsOperator::raiseEvent(eCMDEvents CMDEvent, pktData pktBody)
//---------------------------------------------------------------------------------------------
{
	typedef cCMDEventsOperator _callback;

	pCMDEvt pEvent = new cCMDEvent();

	pEvent->eventType = EVENTTYPE_CMD;
	pEvent->CMDEvent = CMDEvent;
	pEvent->CMDArgs = pktBody;

	// Detailed event customization
	switch(CMDEvent)
	{
	case EVENT_CMD_ONLOGIN:
		pEvent->getDelegate()->addRef(this, _callback::onLogin);
		break;

	case EVENT_CMD_ONCREATEROOM:
		pEvent->getDelegate()->addRef(this, _callback::onCreateRoom);
		break;

	case EVENT_CMD_ONPART:
		pEvent->getDelegate()->addRef(this, _callback::onPart);
		break;

	case EVENT_CMD_ONJOIN:
		pEvent->getDelegate()->addRef(this, _callback::onJoin);
		break;

	case EVENT_CMD_ONQUIT:
		pEvent->getDelegate()->addRef(this, _callback::onQuit);
		break;

	case EVENT_CMD_ONLIST:
		pEvent->getDelegate()->addRef(this, _callback::onList);
		break;

	case EVENT_CMD_ONREADY:
		pEvent->getDelegate()->addRef(this, _callback::onReady);
		break;

	case EVENT_CMD_ONMESSAGE:
		pEvent->getDelegate()->addRef(this, _callback::onMsg);
		break;

	case EVENT_CMD_ONEVENT:
		pEvent->getDelegate()->addRef(this, _callback::onEvent);
		break;

	}

	pEventProcessor->addItem(pEvent);
}

__END_SPK_NAMESPACE