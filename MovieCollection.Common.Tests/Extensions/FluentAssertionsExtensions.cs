using FluentAssertions;
using FluentAssertions.Collections;
using MovieCollection.Infrastructure.DTOs;

namespace MovieCollection.Common.Tests.Extensions
{
    public static class FluentAssertionsExtensions
    {
        public static AndWhichConstraint<GenericCollectionAssertions<AppMessage>, AppMessage> Contain(
            this GenericCollectionAssertions<AppMessage> genericCollectionAssertions, 
            string key, 
            string value)
        {
            return genericCollectionAssertions.ContainSingle(message => message.Code == key && message.Description == value);
        }

        public static AndWhichConstraint<GenericCollectionAssertions<AppMessage>, AppMessage> ContainKey(
            this GenericCollectionAssertions<AppMessage> genericCollectionAssertions, 
            string key)
        {
            return genericCollectionAssertions.ContainSingle(message => message.Code == key);
        }
    }
}
