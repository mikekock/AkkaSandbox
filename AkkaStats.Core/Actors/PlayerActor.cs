using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Akka;
using Akka.Actor;
using AkkaStats.Core.Messages;
using AkkaStats.Core.Events;
using Akka.Persistence;

namespace AkkaStats.Core
{
    public class HitterActor : AggregateRoot<Hitter>
    {
        private readonly Guid _id;
        //private Hitter hitter;

        public HitterActor(Guid id)
            : base("hitter-" + id.ToString("N"))
        {
            _id = id;
            Context.Become(Active);
            //            Context.Become(Uninitialized);
        }

        /*/// <summary>
        /// Gets total amount of funds reserved on pending transactions authentication.
        /// </summary>
        public decimal ReservedFunds
        {
            get { return _pendingTransactions.Aggregate(0M, (acc, transaction) => acc + transaction.Amount); }
        }*/


        protected override bool OnCommand(object message)
        {
            return false;
        }

        protected override void OnReplaySuccess()
        {
            /*if (State == null)
            {
                //Become(Uninitialized);
            }
            else if (State.IsActive)
            {
                Become(Active);
            }
            else
            {
                //Become(Deactivated);
            }*/
            
        }

        
        protected override void Unhandled(object message)
        {
            base.Unhandled(message);
        }

        protected override void OnReplayFailure(Exception ex)
        {
            string s = ex.Message;
        }

        protected override void UpdateState(IEvent domainEvent, IActorRef sender)
        {
            domainEvent.Match()
                .With<HitterAddedEvent>(e => State = new Hitter(e.Id, e.Name))
                .With<HomeRunHitEvent>(e => State.HitHomeRun());
            /*domainEvent.Match()
                .With<AccountEvents.Withdrawal>(e => State.Balance -= e.Amount)
                .With<AccountEvents.Deposited>(e => State.Balance += e.Amount)
                .With<AccountEvents.TransferedWithdrawal>(e => State.Balance -= e.Amount)
                .With<AccountEvents.TransferedDeposit>(e => State.Balance += e.Amount)
                .With<AccountEvents.AccountCreated>(e =>
                {
                    State = new AccountEntity(e.Id, e.OwnerId, true, e.Balance);
                    Context.Become(Active);
                    if (sender != null) sender.Tell(State);

                    Log.Info("Account with id {0} and balance {1} has been created", e.Id, e.Balance);
                })
                .With<AccountEvents.AccountDeactivated>(e =>
                {
                    State.IsActive = false;
                    Context.Become(Deactivated);

                    Log.Info("Account with id {0} has been deactivated", e.Id);
                });*/
        }

        /*private bool Uninitialized(object message)
        {
            return message.Match()
                .With<AccountCommands.CreateAccount>(create =>
                {
                    Persist(new AccountEvents.AccountCreated(_id, create.OwnerId, create.Balance, Clock()), Sender);
                })
                .WasHandled;
        }*/

        private bool Active(object message)
        {
            //return base.ReceiveCommand(message) || message.Match().With<HitterMessage>(ProcessHitterMessage)
            return base.ReceiveCommand(message) || message.Match()
                .With<CreateHitterMessage>(CreateHitter)
                .With<HitHomeRunMessage>(HitHomeRun)
                .WasHandled;
            /*return base.ReceiveCommand(message) || message.Match()
                .With<TransactionCoordinator.BeginTransaction>(EstablishTransferTransaction)
                .With<TransactionCoordinator.Commit>(CommitTransfer)
                .With<TransactionCoordinator.Rollback>(AbortTransfer)
                .With<AccountCommands.DeactivateAccount>(Deactivate)
                .With<AccountCommands.Deposit>(Deposit)
                .With<AccountCommands.Withdraw>(Withdraw)
                .WasHandled;*/
        }

        private void CreateHitter(CreateHitterMessage message)
        {
            Persist(new HitterAddedEvent(message.Id, message.Name));
            //if (message.State == AkkaStats.Core.Messages.CRUDState.Read) // && message.Id == _id)
            {

                //State = new Hitter(_id, message.Name);
                /*for (int i = 0; i < message.Hrs; i++)
                    State.HitHomeRun();
                State.MoveWebsite(message.Url);*/
            }
        }

        private void HitHomeRun(HitHomeRunMessage message)
        {
            //State.HitHomeRun();
            Persist(new HomeRunHitEvent());
        }
        /*
        private void Deactivate(AccountCommands.DeactivateAccount deactivate)
        {
            Persist(new AccountEvents.AccountDeactivated(deactivate.AccountId, Clock()));
        }

        private void Deposit(AccountCommands.Deposit deposit)
        {
            if (deposit.Amount > 0)
            {
                Persist(new AccountEvents.Deposited(_id, deposit.Amount, Clock()));
            }
            else
            {
                Log.Error("Cannot perform deposit on account {0}: money amount is not positive value", _id);
            }
        }

        private void Withdraw(AccountCommands.Withdraw withdraw)
        {
            var sender = Sender;
            var withdrawal = new AccountEvents.Withdrawal(_id, withdraw.Amount, Clock());

            // Use defer to await to proceed command until all account events have been
            // persisted and handled. This is done mostly, because we don't want to perform
            // negative account check while there may be still account balance modifying events
            // waiting in mailbox.
            Defer(withdrawal, e =>
            {
                if (withdraw.Amount > 0 && withdraw.Amount <= (State.Balance - ReservedFunds))
                {
                    Persist(e, sender);
                }
                else
                {
                    Log.Error("Cannot perform withdrawal from account {0}, because it has a negative balance", _id);
                    sender.Tell(new NotEnoughtFunds(_id));
                }
            });
        }

        /// <summary>
        /// Aborts related transaction and sends <see cref="TransactionCoordinator.Ack"/> back to transaction coordinator.
        /// </summary>
        private void AbortTransfer(TransactionCoordinator.Rollback e)
        {
            var abortedTransaction = _pendingTransactions.FirstOrDefault(tx => tx.TransactionId == e.TransactionId);
            if (abortedTransaction != null)
            {
                _pendingTransactions.Remove(abortedTransaction);
                Sender.Tell(new TransactionCoordinator.Ack(e.TransactionId));
            }
            else Unhandled(e);
        }

        /// <summary>
        /// Commits target transaction, persisting a transaction event and sending <see cref="TransactionCoordinator.Ack"/> to transaction coordinator.
        /// </summary>
        private void CommitTransfer(TransactionCoordinator.Commit e)
        {
            var transaction = _pendingTransactions.SingleOrDefault(tx => tx.TransactionId == e.TransactionId);
            if (transaction != null)
            {
                // apply pending transaction and confirm operation
                var transfered = Self.Equals(transaction.Sender)
                    ? (IAccountEvent)
                        new AccountEvents.TransferedWithdrawal(State.Id, transaction.TransactionId, transaction.Amount, Clock())
                    : new AccountEvents.TransferedDeposit(State.Id, transaction.TransactionId, transaction.Amount, Clock());

                Persist(transfered);

                // don't send ACK until you're sure, that event has been stored
                Defer(transfered, _ =>
                {
                    _pendingTransactions.Remove(transaction);
                    Sender.Tell(new TransactionCoordinator.Ack(transaction.TransactionId));
                });
            }
            else Unhandled(e);
        }

        /// <summary>
        /// Establishes first phase of the two-phase commit transaction. Current account funds are being verified. If transfer can be proceed,
        /// transaction goes onto pending transactions list nad <see cref="TransactionCoordinator.Commit"/> message is sent to transaction coordinator.
        /// Otherwise transaction is aborted.
        /// </summary>
        private void EstablishTransferTransaction(TransactionCoordinator.BeginTransaction e)
        {
            var pendingTransaction = e.Payload as PendingTransfer;
            if (pendingTransaction != null)
            {
                if (Self.Equals(pendingTransaction.Sender))
                {
                    // if current actor is account sender, 
                    var unreserved = State.Balance - ReservedFunds;
                    if (pendingTransaction.Amount > 0 && pendingTransaction.Amount <= unreserved)
                    {
                        _pendingTransactions.Add(pendingTransaction);
                        Sender.Tell(new TransactionCoordinator.Continue(pendingTransaction.TransactionId));
                    }
                    else
                    {
                        Sender.Tell(new TransactionCoordinator.Abort(pendingTransaction.TransactionId,
                            new Exception(string.Format("Account {0} has insufficient funds. Unreserved balance {1}, requested {2}", State.Id, unreserved, pendingTransaction.Amount))));
                    }
                }
                else if (Self.Equals(pendingTransaction.Recipient))
                {
                    // recipient's account doesn't need to check if it has enough funds
                    _pendingTransactions.Add(pendingTransaction);
                    Sender.Tell(new TransactionCoordinator.Commit(pendingTransaction.TransactionId));
                }
                else
                {
                    Sender.Tell(new TransactionCoordinator.Abort(e.TransactionId,
                        new Exception(string.Format(
                            "Transaction {0} was addressed to {1}, who is neither sender nor recipient", e.TransactionId, Self))));
                    Unhandled(e);
                }
            }
        }
        */

        //private bool Deactivated(object message)
        //{
        //    return base.ReceiveCommand(message) || message.Match()
        //        .With<AccountCommands.DeactivateAccount>(_ => { /* ignore */ })
        //        .WasHandled;
        //}
    }


    public class HitterHomeRunView : PersistentView
    {
        #region messages

        
        public sealed class GetLastHomeRunInsertedDateTime : ICommand
        {
            public GetLastHomeRunInsertedDateTime(Guid id)
            {
                Id = id;
            }

            public Guid Id { get; private set; }
        }

        #endregion

        private readonly Guid _id;

        // you would probably like to record this directly in database
        protected DateTime LastHomeRunInsertedDateTime = new DateTime(1980, 1, 1);

        public HitterHomeRunView(Guid id)
        {
            _id = id;
            
        }

        /// <summary>
        /// View id is unique identifier of current view. It's used i.e. for persistent view state snapshotting.
        /// </summary>
        public override string ViewId { get { return "hitter-view-" + _id.ToString("N"); } }

        /// <summary>
        /// Persistence id is used to identify aggregate root (<see cref="Account"/> instance in 
        /// this case) which will be used as an event source for current persistent view.
        /// </summary>
        public override string PersistenceId { get { return "hitter-" + _id.ToString("N"); } }
        private int HomeRunCount = 0;

        protected override bool Receive(object message)
        {
            return message.Match()
                /*.With<GetHistoryPage>(page =>
                {
                    Sender.Tell(History.Skip(page.Skip).Take(page.Take).ToArray(), Self);
                })*/
                .With<HitterAddedEvent>(e =>
                {
                    HomeRunCount = 0;
                })
                .With<HomeRunHitEvent>(e =>
                {
                    LastHomeRunInsertedDateTime = DateTime.Now;
                    ++HomeRunCount;
                    //RecordWithdrawal(e.FromId, e.Amount, e.Timestamp);
                })
                .With<GetLastHomeRunInsertedDateTime>(e =>
                {
                    Sender.Tell(LastHomeRunInsertedDateTime, Self);
                })
                .WasHandled;
        }
    }


}
