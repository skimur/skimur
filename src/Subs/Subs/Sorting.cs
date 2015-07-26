using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Subs
{
    public class Sorting
    {
        private static readonly double[] CachedConfidences;
        private static readonly int CachedUpRange = 400;
        private static readonly int CachedDownRange = 100;

        static Sorting()
        {
            CachedConfidences = new double[CachedDownRange*CachedUpRange];
            for (var ups = 0; ups < CachedUpRange; ups++)
                for (var downs = 0; downs < CachedDownRange; downs++)
                    CachedConfidences[downs + ups * CachedDownRange] = Confidence(ups, downs);         
        }

        public static double Hot(int hot, long timeStamp)
        {
            var order = Math.Log10(Math.Max(Math.Abs(hot), 1));

            int sign;

            if (hot > 0)
                sign = 1;
            else if (hot < 0)
                sign = -1;
            else
                sign = 0;

            return Math.Round(sign * order + (timeStamp - 1134028003) / 45000.0, 7);
        }

        public static int Score(int ups, int downs)
        {
            return ups - downs;
        }

        public static double GetHotFactor(int hot, long now, long timestamp, double? ageWeight)
        {
            if (ageWeight == null)
                ageWeight = 0;

            return Math.Max(hot + ((now - timestamp) * ageWeight.Value) / 45000.0, 1);
        }

        public static double Controversy(int ups, int downs)
        {
            if (downs <= 0 || ups <= 0)
                return 0;

            return Math.Pow(ups + downs, ups > downs ? (float) downs/ups : (float) ups/downs);
        }

        public static double Confidence(int ups, int downs)
        {
            var n = (ups + downs);

            if (n == 0)
                return 0;

            var z = 1.281551565545D; // 80% confidence
            var p = (double)ups/n;

            var left = p + 1D/(2*n)*z*z;
            var right = z*Math.Sqrt(p*(1 - p)/n + z*z/(4*n*n));
            var under = 1 + 1D/n*z*z;

            return (left - right)/under;
        }

        public static double CachedConfidence(int ups, int downs)
        {
            if (ups + downs == 0)
                return 0;
            if (ups < CachedUpRange && downs < CachedDownRange)
                return CachedConfidences[downs + ups*CachedDownRange];
            return Confidence(ups, downs);
        }

        public static double Qa(double questionScore, int questionLength, double answerScore = 0, int answerLength = 1)
        {
            var scoreModifier = questionScore + answerScore;

            var lengthModifier = Math.Log10(questionLength + answerLength);

            return scoreModifier + (lengthModifier/5);
        }

        public static double Qa(int questionUps, int questionDowns, int questionLength, List<Comment> opChildren)
        {
            var questionScore = Confidence(questionUps, questionDowns);

            double? bestScore = null;
            var answerLength = 1;

            if (opChildren != null)
            {
                foreach (var answer in opChildren)
                {
                    var score = Confidence(answer.VoteUpCount, answer.VoteDownCount);
                    if (!bestScore.HasValue || score > bestScore)
                    {
                        bestScore = score;
                        answerLength = !string.IsNullOrEmpty(answer.Body) ? answer.Body.Length : 0;
                    }
                }
            }

            return bestScore.HasValue ? Qa(questionScore, questionLength, bestScore.Value, answerLength) : Qa(questionScore, questionLength);
        }
    }
}
