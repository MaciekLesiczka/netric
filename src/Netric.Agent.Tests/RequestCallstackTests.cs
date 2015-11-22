using System;
using System.Collections.Generic;
using System.Linq;
using Netric.Agent.Clr;
using Xunit;

namespace Netric.Agent.Tests
{
    public class RequestCallstackTests
    {
        private readonly RequestCallStack _target;

        public RequestCallstackTests()
        {
            _target = new RequestCallStack();
        }

        [Fact]
        public void Should_return_methods_in_enter_order()
        {
            //arrange
            _target.RegisterMethodEnter(new MethodEnter() {CallId = 1, Name = "a"});
            _target.RegisterMethodEnter(new MethodEnter() {CallId = 2, Name = "b"});
            _target.RegisterMethodEnter(new MethodEnter() {CallId = 3, Name = "c"});

            _target.RegisterMethodLeave(new MethodLeave() { CallId = 3, Name = "c" });
            _target.RegisterMethodLeave(new MethodLeave() { CallId = 2, Name = "b" });
            _target.RegisterMethodLeave(new MethodLeave() { CallId = 1, Name = "a" });

            //act
            var methods = _target.Select(m => m.Name).ToList();

            Assert.Equal(new List<string> {"a", "b", "c"}, methods);
        }

        [Fact]
        public void RegisterMethodEnter_prevents_from_registration_in_invalid_order()
        {
            //arrange
            _target.RegisterMethodEnter(new MethodEnter() { CallId = 5, Name = "a" });
            _target.RegisterMethodEnter(new MethodEnter() { CallId = 6, Name = "b" });
            
            //act
            var result = Assert.Throws<ArgumentException>(() => _target.RegisterMethodEnter(new MethodEnter() { CallId = 2, Name = "c" }));
            //assert
            Assert.Equal("Methods must be registered in call order. Last method callId: 6, callId attempt:2",result.Message);
        }

        [Fact]
        public void RegisterMethodEnter_prevents_from_callId_to_be_lower_than_zero()
        {            
            //act
            var result = Assert.Throws<ArgumentException>(() => _target.RegisterMethodEnter(new MethodEnter() { CallId = -1, Name = "c" }));
            //assert
            Assert.Equal("CallId cannot be lower than zero", result.Message);
        }
    }
}