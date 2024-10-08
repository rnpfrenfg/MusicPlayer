#pragma once

#include <vector>

enum class EventType
{
	ON_MOVE,
	GAME_END,

	NUM_EVENTS
};

class EventListener
{
public:
	virtual ~EventListener() {};

	virtual void HandleEvent(EventType type, void* pEventData) = 0;
};

namespace Event {
	enum { CNUM_EVENTS = static_cast<int>(EventType::NUM_EVENTS) };
}

class EventManager
{
public:
	EventManager() = default;
	~EventManager() = default;

	void TriggerEvent(EventType type, void* eventData);

	void AddListener(EventListener* listener, EventType type);
	void RemoveListener(EventListener* listener, EventType type);
	void RemoveAll(EventListener* pListener);

	void AddEventToQueue(EventType type, void* eventData);
	void RunQueue(EventType type);

private:
	EventManager(const EventManager&) = default;
	EventManager& operator=(const EventManager&) = default;

	std::vector<EventListener*> mListenersVec[Event::CNUM_EVENTS];
	std::vector<void*> mEventQueue[Event::CNUM_EVENTS];
};