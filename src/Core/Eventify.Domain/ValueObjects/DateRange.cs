using Eventify.Domain.Common;

namespace Eventify.Domain.ValueObjects
{
    public class DateRange : ValueObject
    {
        public DateTime StartDate { get; private set; }
        public DateTime EndDate { get; private set; }

        private DateRange() { } // EF Core

        public DateRange(DateTime startDate, DateTime endDate)
        {
            if (startDate >= endDate)
                throw new ArgumentException("Start date must be before end date");

            StartDate = startDate;
            EndDate = endDate;
        }

        public int DurationInDays => (EndDate - StartDate).Days;
        public TimeSpan Duration => EndDate - StartDate;

        public bool Contains(DateTime date)
        {
            return date >= StartDate && date <= EndDate;
        }

        public bool Overlaps(DateRange other)
        {
            return StartDate <= other.EndDate && EndDate >= other.StartDate;
        }

        protected override IEnumerable<object> GetEqualityComponents()
        {
            yield return StartDate;
            yield return EndDate;
        }

        public override string ToString()
        {
            return $"{StartDate:yyyy-MM-dd} to {EndDate:yyyy-MM-dd}";
        }
    }
}
