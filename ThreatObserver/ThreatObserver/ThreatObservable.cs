using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using ThreatInterface;

namespace ThreatObserver
{
    public class ThreatObservable : Actor, ISubject
    {
        private ObservableCollection<IIssueObserver> _observers;
        private ThreatState _threatModel;

        public ThreatObservable()
        {
            _observers = new ObservableCollection<IIssueObserver>();
        }

        public async Task<bool> RegisterObserver(IIssueObserver observer)
        {
            try
            {
                if (!_observers.Contains(observer))
                {
                    _observers.Add(observer);
                }

            }
            catch (Exception ex)
            {
                Trace.WriteLine("Exception occured in Register Object {0}", ex.InnerException.Message);
                return false;
            }
            
            return true;
        }

        public async Task<bool> RemoveObserver(IIssueObserver observer)
        {
            try
            {
                int index = _observers.IndexOf(observer);
                if (index >= 0)
                {
                    _observers.RemoveAt(index);
                }
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Exception in RemoveObserver : {0}", ex.InnerException.Message);
                return false;
            }
            return true;
        }

        /// <summary>
        /// Set the State of the Subject
        /// </summary>
        /// <param name="state">Get the state of the Subject</param>
        public async Task<bool> SetState(ThreatState state)
        {
            try
            {
                this._threatModel = state;
                NotifyObserver();
            }
            catch (Exception ex)
            {
                Trace.WriteLine("Exception in Set State : {0}", ex.InnerException.Message);
                return false;
            }
            return true;
        }
        
        /// <summary>
        /// Notify the observers
        /// </summary>
        public void NotifyObserver()
        {
            foreach (var issueObserver in _observers)
            {
                issueObserver.Update(_threatModel);
            }
        }

    }
}
