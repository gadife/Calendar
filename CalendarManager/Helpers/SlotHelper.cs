using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using CalendarManager.Models;

namespace CalendarManager.Helpers
{
    public static class SlotHelper
    {
        public static List<Event> GetFreeSlots(List<string> users, double meetingDuration, DateTime from, DateTime to)
        {
            using (EventsBbEntities context = new EventsBbEntities())
            {
                List<Event> busy = context.Events.Where(e => e.EventUsers.Any(u => users.Contains(u.Email)))
                                                        .OrderBy(e => e.StartTime).ToList();


                busy = GetAllOverlaps(busy).Where(b => b.StartTime >= from && b.EndTime <= to).ToList();
                List<Event> result = new List<Event>();

                if (busy.Count != 0)
                {
                    result.Add(new Event()
                    {
                        StartTime = from,
                        EndTime = busy.First().StartTime
                    });
                    result.Add(new Event()
                    {
                        StartTime = busy.Last().EndTime,
                        EndTime = to,
                    });

                    for (int i = 0; i < busy.Count - 1; i++)
                    {
                        if ((busy[i + 1].StartTime - busy[i].EndTime).TotalHours > meetingDuration)
                        {
                            result.Add(new Event()
                            {
                                StartTime = busy[i].EndTime,
                                EndTime = busy[i + 1].StartTime
                            });
                        }
                    }
                }
                else
                {
                    result.Add(new Event()
                    {
                        StartTime = from,
                        EndTime = to,
                    });
                }

                return result;
            }
        }

        public static List<Event> GetBusyTimes(List<string> users, double meetingDuration, DateTime from, DateTime to)
        {
            using (EventsBbEntities context = new EventsBbEntities())
            {
                List<Event> busy = context.Events.Where(e => e.EventUsers.Any(u => users.Contains(u.Email)))
                                                        .OrderBy(e => e.StartTime).ToList();


                busy = GetAllOverlaps(busy).Where(b => b.StartTime >= from && b.EndTime <= to).ToList();
                return busy;
            }
        }

        public static List<Event> GetBusyTimes(string email)
        {
            EventsBbEntities context = null;
            List<Event> result = new List<Event>();

            //using (EventsBbEntities context = new EventsBbEntities())
            try
            {
                context = new EventsBbEntities();

                List<Event> events = context.Events.Where(e => e.EventUsers.Any(u => u.Email == email))
                                                   .OrderBy(e => e.StartTime).ToList();

                //result = GetAllOverlaps(events);
                result = GetAllOverlaps2(events);
            }
            catch (Exception e)
            {
                Logger.Error(e, "An error as accured while calculating busy time of {0}", email);
            }
            finally
            {
                if (context != null)
                {
                    context.Dispose();
                }
            }

            return result;
        }

        private static List<Event> GetFreeSlots(List<Event> busyTimes, double duration)
        {
            List<Event> result = new List<Event>();
            for (int i = 0; i < busyTimes.Count() - 1; i++)
            {
                if ((busyTimes[i + 1].StartTime - busyTimes[i].EndTime).TotalHours > duration)
                {
                    result.Add(new Event()
                    {
                        StartTime = busyTimes[i].EndTime,
                        EndTime = busyTimes[i + 1].StartTime
                    });
                }
            }

            return result;
        }

        private static List<Event> GetAllOverlaps(List<Event> events)
        {
            int j = 0;
            List<Event> result = new List<Event>();
            Event temp = null;
            for (int i = 0; i < events.Count - 1; i++)
            {
                Event overlap = null;
                temp = GetOverlap(events[i], events[i + 1]);
                j = i;

                if (temp == null)
                {
                    overlap = events[i];
                }
                else
                {
                    j++;
                    overlap = temp;
                }

                while (temp != null)
                {
                    if (j == events.Count - 1)
                    {
                        break;
                    }
                    else
                    {
                        j++;
                        while (IsBetween(events[i], events[j]))
                        {
                            j++;
                        }


                        temp = GetOverlap(overlap, events[j]);

                        if (temp != null)
                        {
                            overlap = temp;
                        }
                        else
                        {
                            j--;
                        }
                    }
                }
                i = j;

                result.Add(overlap);
            }

            if (temp == null && events.Count > 0)
            {
                result.Add(events.Last());
            }

            return result;
        }

        private static Event GetOverlap(Event first, Event second)
        {
            Event result = null;

            if (IsDateOverlap(first, second))
            {
                result = new Event()
                {
                    StartTime = first.StartTime,
                    EndTime = second.EndTime
                };
            }

            return result;
        }

        private static bool IsDateOverlap(Event first, Event second)
        {
            return second.EndTime.Ticks >= first.EndTime.Ticks &&
                   second.StartTime.Ticks <= first.EndTime.Ticks;
        }

        private static bool IsBetween(Event first, Event second)
        {
            return second.EndTime.Ticks <= first.EndTime.Ticks &&
                   second.StartTime.Ticks >= first.StartTime.Ticks;
        }

        private static bool IsStartBetween(Event first, Event second)
        {
            return first.StartTime.Ticks <= second.StartTime.Ticks &&
                   first.EndTime.Ticks >= second.StartTime.Ticks;
        }

        private static List<Event> GetAllOverlaps2(List<Event> sortedEvents)
        {
            List<Event> busy = new List<Event>();
            for (int i = 0; i < sortedEvents.Count - 1; i++)
            {
                Event temp = sortedEvents[i];

                while (i+1 < sortedEvents.Count && IsStartBetween(temp, sortedEvents[i + 1]))
                {
                    temp.StartTime = temp.StartTime;
                    temp.EndTime = temp.EndTime > sortedEvents[i + 1].EndTime ? temp.EndTime : sortedEvents[i + 1].EndTime;

                    i++;
                }

                busy.Add(temp);
            }

            return busy;
        }
    }
}