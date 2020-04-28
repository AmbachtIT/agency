using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;

namespace Agency.Test.Performance
{
    
    /// <summary>
    /// Tests the performance impact of different types of method invocation
    /// </summary>
    public class MethodInvocation
    {

        [Test(), Explicit()]
        public void Test()
        {
            var invocations = 100000;
            var random = new Random(190681);
            var opCount = 3;
            var start = random.Next(100000);
            var ops = 
                Enumerable
                    .Range(0, invocations).Select(_ => random.Next(opCount))
                    .ToArray();
            
            var check = start;
            foreach (var op in ops)
            {
                check = Perform(check, op);
            }

            foreach (var strategy in GetStrategies().Concat(GetStrategies().Reverse()))
            {
                strategy.Init(opCount);
                Run(strategy, ops,  start, check);
            }
            


        }

        private void Run(InvocationStrategy strategy, int[] ops, int start, int check)
        {
            TestContext.Out.WriteLine(strategy.GetType().Name);
            for (var i = 0; i < 4; i++)
            {
                var value = start;
                var startTime = DateTime.UtcNow;
                value = strategy.Invoke(ops, value);
                var duration = DateTime.UtcNow - startTime;
                if (i > 0) // 0 == warmup
                {
                    TestContext.Out.WriteLine($" - Run {i}: {duration.TotalMilliseconds}ms");
                }
                
                Assert.AreEqual(check, value);
            }
            TestContext.Out.WriteLine();
        }

        private IEnumerable<InvocationStrategy> GetStrategies()
        {
            yield return new PublicStaticInvocationStrategy();
            yield return new InterfaceInvocationStrategy();
            yield return new InterfaceFuncInvocationStrategy();
        }


        private static int Perform(int result, int operation)
        {
            switch (operation)
            {
                case 0:
                    return Op0(result);
                
                case 1:
                    return Op1(result);
                
                case 2:
                    return Op2(result);
                
                default:
                    throw new InvalidOperationException();
            }
        }
        
        public static int Op0(int result)
        {
            return result + 1;
        }
        public static int Op1(int result)
        {
            return result - 1;
        }
        public static int Op2(int result)
        {
            if (result % 2 == 0)
            {
                return result + 2;
            }
            return result - 2;
        }
        

        private abstract class InvocationStrategy
        {
            public abstract void Init(int typeCount);

            public abstract int Invoke(int[] types, int value);
        }

        private class PublicStaticInvocationStrategy : InvocationStrategy
        {
            public override void Init(int typeCount)
            {
            }

            public override int Invoke(int[] types, int value)
            {
                for (var i = 0; i < types.Length; i++)
                {
                    var type = types[i];
                    switch (type)
                    {
                        case 0:
                            value = Op0(value);
                            break;
                        case 1:
                            value = Op1(value);
                            break;
                        case 2:
                            value = Op2(value);
                            break;
                        default:
                            throw new InvalidOperationException();
                    }
                }
                return value;
            }


        }

        private class PrivateStaticInvocationStrategy : InvocationStrategy
        {
            public override void Init(int typeCount)
            {
            }

            public override int Invoke(int[] types, int value)
            {
                for (var i = 0; i < types.Length; i++)
                {
                    var type = types[i];
                    switch (type)
                    {
                        case 0:
                            value = Op0(value);
                            break;
                        case 1:
                            value = Op1(value);
                            break;
                        case 2:
                            value = Op2(value);
                            break;
                        default:
                            throw new InvalidOperationException();
                    }
                }
                return value;
            }


        }

        
        private class InterfaceInvocationStrategy : InvocationStrategy
        {
            public override void Init(int typeCount)
            {
            }

            private readonly IOperation[] ops = new IOperation[]
            {
                new COp0(),
                new COp1(),
                new COp2(),
            };
            
            public override int Invoke(int[] types, int value)
            {
                for (var i = 0; i < types.Length; i++)
                {
                    var type = types[i];
                    value = ops[type].Perform(value);
                }
                return value;
            }
            

            private class COp2 : IOperation
            {
                public int Perform(int value)
                {
                    if (value % 2 == 0)
                    {
                        return value + 2;
                    }
                    return value - 2;
                }
            } 
        }
        
        private class InterfaceFuncInvocationStrategy : InvocationStrategy
        {
            public override void Init(int typeCount)
            {
                funcs = new Func<int, int>[]
                {
                    ops[0].Perform,
                    ops[1].Perform,
                    ops[2].Perform,
                };
            }

            private readonly IOperation[] ops = new IOperation[]
            {
                new COp0(),
                new COp1(),
                new COp2(),
            };

            private Func<int, int>[] funcs;
            
            public override int Invoke(int[] types, int value)
            {
                for (var i = 0; i < types.Length; i++)
                {
                    var type = types[i];
                    value = funcs[type](value);
                }
                return value;
            }
            

            private class COp2 : IOperation
            {
                public int Perform(int value)
                {
                    if (value % 2 == 0)
                    {
                        return value + 2;
                    }
                    return value - 2;
                }
            } 
        }
        
        private interface IOperation
        {
            int Perform(int value);
        }

        private class COp0 : IOperation
        {
            public int Perform(int value)
            {
                return value + 1;
            }
        } 
        private class COp1 : IOperation
        {
            public int Perform(int value)
            {
                return value - 1;
            }
        } 
        
        
        
    }
}