using System;
using Stateless;

namespace BugPro
{
    public enum BugState
    {
        New,
        Triage,
        InProgress,
        NeedsMoreInfo,
        NotReproducible,
        NoTime,
        NotABug,
        Fixed,
        Reopened,
        Closed,
        Rejected
    }

    public enum BugAction
    {
        Submit,
        ProvideMoreInfo,
        AssignToDev,
        MarkNotABug,
        RequestMoreInfo,
        MarkNoTime,
        MarkFixed,
        MarkNotReproducible,
        MarkNoTimeDev,
        VerifyAndClose,
        Reopen,
        ReopenFromClosed,
        SendToTriage,
        Close
    }

    public class Bug
    {
        private StateMachine<BugState, BugAction> _machine;
        private BugState _state;

        public BugState CurrentState => _state;

        public Bug()
        {
            _machine = new StateMachine<BugState, BugAction>(BugState.New);

            _machine.Configure(BugState.New)
                .Permit(BugAction.Submit, BugState.Triage);

            _machine.Configure(BugState.Triage)
                .Permit(BugAction.AssignToDev, BugState.InProgress)
                .Permit(BugAction.MarkNotABug, BugState.NotABug)
                .Permit(BugAction.RequestMoreInfo, BugState.NeedsMoreInfo)
                .Permit(BugAction.MarkNoTime, BugState.NoTime);

            _machine.Configure(BugState.InProgress)
                .Permit(BugAction.MarkFixed, BugState.Fixed)
                .Permit(BugAction.MarkNotReproducible, BugState.NotReproducible)
                .Permit(BugAction.MarkNoTimeDev, BugState.NoTime);

            _machine.Configure(BugState.NeedsMoreInfo)
                .Permit(BugAction.ProvideMoreInfo, BugState.Triage);

            _machine.Configure(BugState.NotReproducible)
                .Permit(BugAction.VerifyAndClose, BugState.Closed)
                .Permit(BugAction.Reopen, BugState.Triage);

            _machine.Configure(BugState.NoTime)
                .Permit(BugAction.AssignToDev, BugState.InProgress)
                .Permit(BugAction.RequestMoreInfo, BugState.NeedsMoreInfo);

            _machine.Configure(BugState.NotABug)
                .Permit(BugAction.VerifyAndClose, BugState.Closed)
                .Permit(BugAction.Reopen, BugState.Triage);

            _machine.Configure(BugState.Fixed)
                .Permit(BugAction.VerifyAndClose, BugState.Closed)
                .Permit(BugAction.Reopen, BugState.Triage);

            _machine.Configure(BugState.Closed)
                .Permit(BugAction.ReopenFromClosed, BugState.Reopened);

            _machine.Configure(BugState.Reopened)
                .Permit(BugAction.SendToTriage, BugState.Triage);

            _machine.OnTransitioned(transition =>
            {
                _state = transition.Destination;
                Console.WriteLine($"{transition.Source} -> {transition.Destination}");
            });
        }

        public void SubmitNewBug() => _machine.Fire(BugAction.Submit);
        public void ProvideMoreInformation() => _machine.Fire(BugAction.ProvideMoreInfo);
        public void VerifyAndCloseBug() => _machine.Fire(BugAction.VerifyAndClose);
        public void ReopenBug() => _machine.Fire(BugAction.Reopen);
        public void ReopenFromClosed() => _machine.Fire(BugAction.ReopenFromClosed);
        public void SendToTriage() => _machine.Fire(BugAction.SendToTriage);
        public void AssignToDeveloper() => _machine.Fire(BugAction.AssignToDev);
        public void MarkAsNotABug() => _machine.Fire(BugAction.MarkNotABug);
        public void RequestMoreInformation() => _machine.Fire(BugAction.RequestMoreInfo);
        public void MarkAsNoTime() => _machine.Fire(BugAction.MarkNoTime);
        public void MarkAsFixed() => _machine.Fire(BugAction.MarkFixed);
        public void MarkAsNotReproducible() => _machine.Fire(BugAction.MarkNotReproducible);
        public void MarkAsNoTimeDev() => _machine.Fire(BugAction.MarkNoTimeDev);

        public bool CanSubmit() => _machine.CanFire(BugAction.Submit);
        public bool CanProvideMoreInfo() => _machine.CanFire(BugAction.ProvideMoreInfo);
        public bool CanVerifyAndClose() => _machine.CanFire(BugAction.VerifyAndClose);
        public bool CanReopen() => _machine.CanFire(BugAction.Reopen);
        public bool CanReopenFromClosed() => _machine.CanFire(BugAction.ReopenFromClosed);
        public bool CanAssignToDev() => _machine.CanFire(BugAction.AssignToDev);
        public bool CanMarkNotABug() => _machine.CanFire(BugAction.MarkNotABug);
        public bool CanRequestMoreInfo() => _machine.CanFire(BugAction.RequestMoreInfo);
        public bool CanMarkNoTime() => _machine.CanFire(BugAction.MarkNoTime);
        public bool CanMarkFixed() => _machine.CanFire(BugAction.MarkFixed);
        public bool CanMarkNotReproducible() => _machine.CanFire(BugAction.MarkNotReproducible);
        public bool CanMarkNoTimeDev() => _machine.CanFire(BugAction.MarkNoTimeDev);
        public bool CanSendToTriage() => _machine.CanFire(BugAction.SendToTriage);
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Демнострация\n");

            Console.WriteLine("Сц 1: Нормальный путь");
            Bug bug1 = new Bug();
            bug1.SubmitNewBug();
            bug1.AssignToDeveloper();
            bug1.MarkAsFixed();
            bug1.VerifyAndCloseBug();
            Console.WriteLine($"Final state: {bug1.CurrentState}\n");

            Console.WriteLine("Сц 2: не дефект");
            Bug bug2 = new Bug();
            bug2.SubmitNewBug();
            bug2.MarkAsNotABug();
            bug2.VerifyAndCloseBug();
            Console.WriteLine($"Final state: {bug2.CurrentState}\n");

            Console.WriteLine("Сц3: не воспроизводимо");
            Bug bug3 = new Bug();
            bug3.SubmitNewBug();
            bug3.AssignToDeveloper();
            bug3.MarkAsNotReproducible();
            bug3.ReopenBug();
            bug3.AssignToDeveloper();
            bug3.MarkAsFixed();
            bug3.VerifyAndCloseBug();
            Console.WriteLine($"Final state: {bug3.CurrentState}\n");

            Console.WriteLine("Сц 4:Переоткрытие");
            Bug bug4 = new Bug();
            bug4.SubmitNewBug();
            bug4.AssignToDeveloper();
            bug4.MarkAsFixed();
            bug4.VerifyAndCloseBug();
            bug4.ReopenFromClosed();
            bug4.SendToTriage();
            bug4.AssignToDeveloper();
            bug4.MarkAsFixed();
            bug4.VerifyAndCloseBug();
            Console.WriteLine($"Final state: {bug4.CurrentState}\n");

            Console.WriteLine("Сц 5: Доп информация");
            Bug bug5 = new Bug();
            bug5.SubmitNewBug();
            bug5.RequestMoreInformation();
            bug5.ProvideMoreInformation();
            bug5.AssignToDeveloper();
            bug5.MarkAsFixed();
            bug5.VerifyAndCloseBug();
            Console.WriteLine($"Final state: {bug5.CurrentState}\n");

            Console.ReadKey();
        }
    }
}