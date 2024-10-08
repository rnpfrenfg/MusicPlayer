#include "MusicEvent.h"

void EventManager::AddListener(EventListener* listener, EventType type)
{
	mListenersVec[static_cast<int>(type)].push_back(listener);
}

void EventManager::RemoveListener(EventListener* listener, EventType type)
{
	auto& listeners = mListenersVec[static_cast<int>(type)];

	auto result = std::find(listeners.begin(), listeners.end(), listener);

	if (result != listeners.end())
		listeners.erase(result);
}

void EventManager::RemoveAll(EventListener* listener)
{
	int i = 0;
	for (int i = 0; i < Event::CNUM_EVENTS; i++)
	{
		this->RemoveListener(listener, static_cast<EventType>(i));
	}
}

void EventManager::TriggerEvent(EventType type, void* data)
{
	const auto& listeners = mListenersVec[static_cast<int>(type)];

	size_t size = listeners.size();
	for (int i = 0; i < size; i++)
	{
		EventListener* listener = listeners[i];
		listener->HandleEvent(type, data);

		//to check event removed or not...
		size = listeners.size();

		EventListener* temp = listeners[i];
		if (i < size && listener != temp)
		{
			i--;
		}
	}
}

void EventManager::AddEventToQueue(EventType type, void* data) {
	mEventQueue[static_cast<int>(type)].push_back(data);
}

void EventManager::RunQueue(EventType type) {
	auto& queue = mEventQueue[static_cast<int>(type)];
	auto size = queue.size();

	for (int i = 0; i < size; i++) {
		TriggerEvent(type, queue[i]);
	}
	queue.clear();
}