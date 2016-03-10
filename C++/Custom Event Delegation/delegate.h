//=============================================================================================
//					Copyright 2004 Sploosh, All rights reserved.
//
// File: Delegate class (Abstract), base interface of delegation mechanism
//
//=============================================================================================

#ifndef __CDELEGATE_H__
#define __CDELEGATE_H__

__BEG_SPK_NAMESPACE

//---------------------------------------------------------------------------------------------
class ICallback
//---------------------------------------------------------------------------------------------
{
public:
	ICallback();

	virtual ~ICallback();
	virtual void invoke() = 0;
};
//---------------------------------------------------------------------------------------------
template<class AT>
class IDelegate
//---------------------------------------------------------------------------------------------
{
public:
	virtual void invoke(AT* _pArg) = 0;
	virtual void operator ()(AT* _pArg) = 0;
};
//---------------------------------------------------------------------------------------------
template<class R, class CT, class AT>
class IPointer : public IDelegate<AT>
//---------------------------------------------------------------------------------------------
{
public:
	virtual bool isMethod() = 0;
	virtual CT* getObjectPtr() = 0;
	virtual bool operator ==(IPointer<R, CT, AT>* pFunc) = 0;

protected:
	//IPointer() {}
	//virtual ~IPointer() {}
};
//---------------------------------------------------------------------------------------------
template<class R, class CT, class AT>
class cPointerMtd : public IPointer<R, CT, AT>
//---------------------------------------------------------------------------------------------
{	
public:
	cPointerMtd (CT* pObj, R (CT::*pFunc)(AT* _pArg))
	{
		m_pObject = pObj;
		m_pFunction = pFunc;
	}
	
	bool operator ==(IPointer<R, CT, AT>* pFunc)
	{
		if(pFunc->isMethod())
			return false;
		
		cPointerMtd<R, CT, AT>* pFunc1 = (cPointerMtd<R, CT, AT>*)pFunc;
		
		return ((pFunc1->m_pObject == m_pObject) && (pFunc1->m_pFunction == m_pFunction))
			? true : false;
	}
	
	bool isMethod() {return true;}
	void invoke (AT* _pArg) {(m_pObject->*m_pFunction)(_pArg);}
	void operator ()(AT* _pArg)	{invoke (_pArg);}
	CT* getObjectPtr() {return m_pObject;}


private:
	CT* m_pObject;
	R (CT::*m_pFunction)(AT* _pArg);
};
//---------------------------------------------------------------------------------------------
template<class R, class CT, class AT>
class cPointerFunc : public IPointer<R, CT, AT>
//---------------------------------------------------------------------------------------------
{
public:
	cPointerFunc(R (*pFunc)(AT* _pArg))
	{
		m_pFunction = pFunc;
	}

	bool operator ==(IPointer<R, CT, AT>* pFunc)
	{
		if(pFunc->isMethod())
			return false;
		
		cPointerFunc<R, CT, AT>* pFunc1 = (cPointerFunc<R, CT, AT>*)pFunc;
		
		return (pFunc1->m_pFunction == m_pFunction) ? true : false;
	}
	
	bool isMethod()	{return false;}
	void invoke (AT* _pArg) {m_pFunction (_pArg);}
	void operator ()(AT* _pArg)	{invoke (_pArg);}
	CT* getObjectPtr() {return NULL;}

private:
	R (*m_pFunction)(AT* _pArg);
};
//---------------------------------------------------------------------------------------------
// R : Return type
// CT: Object class type
// AT: Function argument type
template<class R, class CT, class AT>
class cDelegate : public IDelegate<AT>
//---------------------------------------------------------------------------------------------
{
public:
	cDelegate() {}

	~cDelegate()
	{
		for(vit i=m_pFuncs.begin(); i!=m_pFuncs.end(); i++)
			delete (*i);

		m_pFuncs.clear();
	}

	void addRef(CT* pObj, R (CT::*pFunc)(AT* _pArg))
	{
		m_pFuncs.push_back(new cPointerMtd<R, CT, AT>(pObj, pFunc));
	}

	void addRef(R (*pFunc)(AT* _pArg))
	{
		m_pFuncs.push_back(new cPointerFunc<R, CT, AT>(pFunc));
	}

	void removeRef(CT* pObj, R (CT::*pFunc)(AT* _pArg))
	{
		cPointerMtd<R, CT, AT> func(pObj, pFunc);

		for(vit i=m_pFuncs.begin(); i!=m_pFuncs.end(); i++)
		{
			if(*(*i) == &func)
			{
				delete *i;
				m_pFuncs.erase(i);
				break;
			}
		}
	}

	void removeRef(R (*pFunc)(AT* _pArg))
	{
		cPointerFunc<R, CT, AT> func(pFunc);

		for(vit i=m_pFuncs.begin(); i!=m_pFuncs.end(); i++)
		{
			if(*(*i) == &func)
			{
				delete *i;
				m_pFuncs.erase(i);
				break;
			}
		}
	}

	//void invoke ()
	//{
	//	for(vit i=m_pFuncs.begin(); i!=m_pFuncs.end(); i++)
	//	{
	//		if ((*i)->isMethod())
	//			invoke((*i)->getObjectPtr());
	//	}
	//}	

	void invoke(AT* _pArg)
	{
		for(vit i=m_pFuncs.begin(); i!=m_pFuncs.end(); i++)
			(*i)->invoke (_pArg);
	}

	void operator ()(AT* _pArg)
	{
		invoke (_pArg);
	}

private:
	vector<IPointer<R, CT, AT>*> m_pFuncs;
	typedef typename vector<IPointer<R, CT, AT>*>::iterator vit;
};

__END_SPK_NAMESPACE

#endif