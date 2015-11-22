using Netric.Agent.Clr;
using Xunit;

namespace Netric.Agent.Tests
{
    public class MethodEnter : IMethodEnter
    {
        public string Name { get; set; }
        public ThreadInfo Thread { get; set; }
        public long CallId { get; set; }
        public long Ticks { get; set; }
    }

    public class MethodLeave : IMethodLeave
    {
        public ThreadInfo Thread { get; set; }
        public string Name { get; set; }
        public long CallId { get; set; }
        public long Ticks { get; set; }
    }


    public class ShadowStackBuilderTests
    {
        [Fact]
        public void Should_calculate_inclusive_time()
        {
            //arrange
            var clr1Call1 = new MethodEnter { CallId = 1, };
            var clr1Call2 = new MethodEnter { CallId = 2, Ticks = 27 };
            var clr1Call3 = new MethodEnter { CallId = 3 };
            var clr2Call3 = new MethodLeave { CallId = 3 };
            var clr1Call4 = new MethodEnter { CallId = 4 };
            var clr2Call4 = new MethodLeave { CallId = 4 };
            var clr2Call2 = new MethodLeave { CallId = 2, Ticks = 40 };
            var clr2Call1 = new MethodLeave { CallId = 1 };

            var target = new ShadowStackBuilder();

            //act
            target.Trace(clr1Call1);
            target.Trace(clr1Call2);
            target.Trace(clr1Call3);
            target.Trace(clr2Call3);
            target.Trace(clr1Call4);
            target.Trace(clr2Call4);
            var result = target.Trace(clr2Call2);
            target.Trace(clr2Call1);

            //assert
            Assert.Equal(13, result.ElapsedInclusive);
        }

        [Fact]
        public void Should_calculate_exclusive_time()
        {
            //arrange
            var clr1Call1 = new MethodEnter { CallId = 1, Ticks = 5 };
            var clr1Call2 = new MethodEnter { CallId = 2, Ticks = 27 };
            var clr1Call3 = new MethodEnter { CallId = 3, Ticks = 30 };
            var clr2Call3 = new MethodLeave { CallId = 3, Ticks = 31 };
            var clr1Call4 = new MethodEnter { CallId = 4, Ticks = 32 };
            var clr2Call4 = new MethodLeave { CallId = 4, Ticks = 35 };
            var clr2Call2 = new MethodLeave { CallId = 2, Ticks = 40 };
            var clr2Call1 = new MethodLeave { CallId = 1, Ticks = 50 };

            var target = new ShadowStackBuilder();

            //act
            target.Trace(clr1Call1);
            target.Trace(clr1Call2);
            target.Trace(clr1Call3);
            target.Trace(clr2Call3);
            target.Trace(clr1Call4);
           var result4 = target.Trace(clr2Call4);
           var result2 = target.Trace(clr2Call2);
           var result1 = target.Trace(clr2Call1);

            //assert
            Assert.Equal(9, result2.ElapsedExclusive);
            Assert.Equal(32, result1.ElapsedExclusive);
            Assert.Equal(3, result4.ElapsedExclusive);

        }
        [Fact]
        public void ShouldCalculateStackLevel()
        {
            //arrange
            var clr1Call1 = new MethodEnter { CallId = 1, };
            var clr1Call2 = new MethodEnter { CallId = 2 };
            var clr1Call3 = new MethodEnter { CallId = 3 };
            var clr2Call3 = new MethodLeave { CallId = 3 };
            var clr1Call4 = new MethodEnter { CallId = 4 };
            var clr2Call4 = new MethodLeave { CallId = 4 };
            var clr2Call2 = new MethodLeave { CallId = 2 };
            var clr2Call1 = new MethodLeave { CallId = 1 };

            var target = new ShadowStackBuilder();

            //act
            target.Trace(clr1Call1);
            target.Trace(clr1Call2);
            target.Trace(clr1Call3);
            var result3 = target.Trace(clr2Call3);
            target.Trace(clr1Call4);
            var result4 = target.Trace(clr2Call4);
            var result2 = target.Trace(clr2Call2);
            var result1 = target.Trace(clr2Call1);

            //assert
            Assert.Equal(2, result3.StackLevel);
            Assert.Equal(2, result4.StackLevel);
            Assert.Equal(1, result2.StackLevel);
            Assert.Equal(0, result1.StackLevel);
        }

        [Fact]
        public void ShouldCalculateStackLevelOnException()
        {
            //arrange
            var clr1Call1 = new MethodEnter { CallId = 1, };
            var clr1Call2 = new MethodEnter { CallId = 2 };
            var clr1Call3 = new MethodEnter { CallId = 3 };

            var clr1Call4 = new MethodEnter { CallId = 4 };
            var clr2Call4 = new MethodLeave { CallId = 4 };
            var clr2Call2 = new MethodLeave { CallId = 2 };
            var clr2Call1 = new MethodLeave { CallId = 1 };

            var target = new ShadowStackBuilder();

            //act
            target.Trace(clr1Call1);
            target.Trace(clr1Call2);
            target.Trace(clr1Call3);

            target.Trace(clr1Call4);
           var result4 = target.Trace(clr2Call4);
           var result2 = target.Trace(clr2Call2);
           var result1 = target.Trace(clr2Call1);

            //assert
            Assert.Equal(3, result4.StackLevel);//we dont know if 4 was called by 2 or by 3
            Assert.Equal(1, result2.StackLevel);
            Assert.Equal(0, result1.StackLevel);
            Assert.True (result2.HandledException);
            Assert.False(result1.HandledException);
            Assert.False(result4.HandledException);
        }

        [Fact]
        public void ShouldCalculateStackLevelOnException2()
        {
            //arrange
            var clr1Call1 = new MethodEnter { CallId = 1, };
            var clr1Call2 = new MethodEnter { CallId = 2 };
            var clr1Call3 = new MethodEnter { CallId = 3 };

            var clr1Call4 = new MethodEnter { CallId = 4 };
            var clr2Call4 = new MethodLeave { CallId = 4 };
            var clr2Call1 = new MethodLeave { CallId = 1 };

            var target = new ShadowStackBuilder();

            //act
            target.Trace(clr1Call1);
            target.Trace(clr1Call2);
            target.Trace(clr1Call3);

            target.Trace(clr1Call4);
            var result4 = target.Trace(clr2Call4);
            var result1 = target.Trace(clr2Call1);

            //assert
            Assert.Equal(3, result4.StackLevel);
            Assert.Equal(0, result1.StackLevel);
            Assert.True(result1.HandledException);
        }

        [Fact]
        public void ShouldCalculateExclusiveTimeOnException()
        {
            //arrange
            var clr1Call1 = new MethodEnter {CallId = 1, Ticks = 5};
            var clr1Call2 = new MethodEnter {CallId = 2, Ticks = 27};
            var clr1Call3 = new MethodEnter {CallId = 3, Ticks = 30};

            var clr1Call4 = new MethodEnter {CallId = 4, Ticks = 32};
            var clr2Call4 = new MethodLeave {CallId = 4, Ticks = 35};
            var clr2Call2 = new MethodLeave {CallId = 2, Ticks = 40};
            var clr2Call1 = new MethodLeave {CallId = 1, Ticks = 50};

            var target = new ShadowStackBuilder();

            //act
            target.Trace(clr1Call1);
            target.Trace(clr1Call2);
            target.Trace(clr1Call3);
            //target.Trace(clr2Call3);
            target.Trace(clr1Call4);
            var result4 = target.Trace(clr2Call4);
            var result2 = target.Trace(clr2Call2);
            var result1 = target.Trace(clr2Call1);

            //assert           
            Assert.Null(result2.ElapsedExclusive); //we dont have full stack information.Unable calculate Exclusive time and it is left null
            Assert.Equal(32, result1.ElapsedExclusive);
            Assert.Equal(3, result4.ElapsedExclusive);
        }
    }
}
