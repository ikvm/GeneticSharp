﻿using System;
using NUnit.Framework;
using NSubstitute;

namespace GeneticSharp.Domain.UnitTests.Chromosomes
{
    [Category("Chromosomes")]
    [TestFixture]
    public class ChromosomeBaseTest
    {
        [Test]
        public void Constructor_InvalidLength_Exception()
        {
            Assert.Catch<ArgumentException>(() =>
            {
                try
                {
                    Substitute.For<ChromosomeBase>(1);
                }
                catch (Exception ex)
                {
                    throw ex.InnerException;
                }
            }, "The minimum length for a chromosome is 2 genes.");
        }

        [Test]
        public void CompareTo_Others_DiffResults()
        {
            var target = Substitute.For<ChromosomeBase>(2);
            target.Fitness = 0.5;

            var other = Substitute.For<ChromosomeBase>(2);
            other.Fitness = 0.5;

            Assert.AreEqual(-1, target.CompareTo(null));
            Assert.AreEqual(0, target.CompareTo(other));

            other.Fitness = 0.4;
            Assert.AreEqual(1, target.CompareTo(other));

            other.Fitness = 0.6;
            Assert.AreEqual(-1, target.CompareTo(other));
        }

        [Test]
        public void Fitness_AnyChange_Null()
        {
            var target = Substitute.For<ChromosomeBase>(2);
            Assert.IsFalse(target.Fitness.HasValue);
            target.Fitness = 0.5;
            Assert.IsTrue(target.Fitness.HasValue);

            target.Fitness = 0.5;
            target.ReplaceGene(0, new Gene(0));
            Assert.IsFalse(target.Fitness.HasValue);

            target.Fitness = 0.5;
            target.ReplaceGenes(0, new Gene[] { new Gene(0) });
            Assert.IsFalse(target.Fitness.HasValue);

            target.Fitness = 0.5;
            target.GenerateGene(0);
            Assert.IsTrue(target.Fitness.HasValue);

            target.Fitness = 0.5;
            target.ReplaceGene(0, new Gene(0));
            Assert.IsFalse(target.Fitness.HasValue);
        }

        [Test]
        public void ReplaceGene_InvalidIndex_Exception()
        {
            var target = Substitute.For<ChromosomeBase>(2);

            Assert.Catch<ArgumentOutOfRangeException>(() =>
            {
                target.ReplaceGene(2, new Gene(0));
            }, "There is no Gene on index 2 to be replaced.");

            Assert.Catch<ArgumentOutOfRangeException>(() =>
            {
                target.ReplaceGene(3, new Gene(0));
            }, "There is no Gene on index 3 to be replaced.");
        }

        [Test]
        public void ReplaceGene_ValidIndex_Replaced()
        {
            var target = Substitute.For<ChromosomeBase>(2);

            target.ReplaceGene(0, new Gene(2));
            target.ReplaceGene(1, new Gene(6));

            var actual = target.GetGenes();
            Assert.AreEqual(2, actual.Length);
            Assert.AreEqual(2, actual[0].Value);
            Assert.AreEqual(6, actual[1].Value);
        }

        [Test]
        public void ReplaceGenes_InvalidIndex_Exception()
        {
            var target = Substitute.For<ChromosomeBase>(2);

            Assert.Catch<ArgumentOutOfRangeException>(() =>
            {
                target.ReplaceGenes(2, new Gene[] { new Gene() });
            }, "There is no Gene on index 2 to be replaced.");

            Assert.Catch<ArgumentOutOfRangeException>(() =>
            {
                target.ReplaceGenes(3, new Gene[] { new Gene() });
            }, "There is no Gene on index 3 to be replaced.");
        }

        [Test]
        public void ReplaceGenes_NullGenes_Exception()
        {
            var target = Substitute.For<ChromosomeBase>(2);

            Assert.Catch<ArgumentNullException>(() =>
            {
                target.ReplaceGenes(0, null);
            });
        }

        [Test]
        public void ReplaceGenes_ValidIndex_Replaced()
        {
            var target = Substitute.For<ChromosomeBase>(4);

            target.ReplaceGenes(0, new Gene[] { new Gene(1), new Gene(2) });

            var actual = target.GetGenes();
            Assert.AreEqual(4, actual.Length);
            Assert.AreEqual(1, actual[0].Value);
            Assert.AreEqual(2, actual[1].Value);

            target.ReplaceGenes(2, new Gene[] { new Gene(3), new Gene(4) });

            actual = target.GetGenes();
            Assert.AreEqual(4, actual.Length);
            Assert.AreEqual(1, actual[0].Value);
            Assert.AreEqual(2, actual[1].Value);
            Assert.AreEqual(3, actual[2].Value);
            Assert.AreEqual(4, actual[3].Value);

            target.ReplaceGenes(3, new Gene[] { new Gene(5) });

            actual = target.GetGenes();
            Assert.AreEqual(4, actual.Length);
            Assert.AreEqual(1, actual[0].Value);
            Assert.AreEqual(2, actual[1].Value);
            Assert.AreEqual(3, actual[2].Value);
            Assert.AreEqual(5, actual[3].Value);
        }

        [Test]
        public void Resize_InvalidLength_Exception()
        {
            var target = Substitute.For<ChromosomeBase>(2);

            Assert.Catch<ArgumentException>(() =>
            {
                target.Resize(1);
            }, "The minimum length for a chromosome is 2 genes.");
        }

        [Test]
        public void Resize_ToLowerLength_TruncateGenes()
        {
            var target = Substitute.For<ChromosomeBase>(4);
            target.ReplaceGenes(0, new Gene[]
            {
                new Gene(1),
                new Gene(2),
                new Gene(3),
                new Gene(4)
            });

            target.Resize(2);
            Assert.AreEqual(2, target.Length);
            Assert.AreEqual(1, target.GetGene(0).Value);
            Assert.AreEqual(2, target.GetGene(1).Value);
        }

        [Test]
        public void Resize_ToGreaterLength_KeepOldGenesAndNullValueNewOnes()
        {
            var target = Substitute.For<ChromosomeBase>(2);
            target.ReplaceGenes(0, new Gene[]
            {
                new Gene(1),
                new Gene(2)
            });

            target.Resize(4);
            Assert.AreEqual(4, target.Length);
            Assert.AreEqual(1, target.GetGene(0).Value);
            Assert.AreEqual(2, target.GetGene(1).Value);
            Assert.IsNull(target.GetGene(2).Value);
            Assert.IsNull(target.GetGene(3).Value);
        }

        [Test]
        public void Equals_NotChromosome_False()
        {
            var target = new ChromosomeStub();
            Assert.IsFalse(target.Equals(1));
        }

        [Test]
        public void GetHashCode_NoFitness_Zero()
        {
            var target = new ChromosomeStub();
            target.Fitness = null;
            Assert.AreEqual(0, target.GetHashCode());
        }

        [Test]
        public void GetHashCode_Fitness_EqualsFitnessHashCode()
        {
            var target = new ChromosomeStub();
            target.Fitness = 123;
            Assert.AreEqual(target.Fitness.GetHashCode(), target.GetHashCode());
        }

        [Test]
        public void OperatorEquals_EqualsReferences_True()
        {
            var first = new ChromosomeStub(1.0);
            var second = first;

            Assert.IsTrue(first == second);
        }

        [Test]
        public void OperatorEquals_AnyNull_False()
        {
            var first = new ChromosomeStub(1.0);
            var second = new ChromosomeStub(1.0);

            Assert.IsFalse(null == second);
            Assert.IsFalse(first == null);
        }

        [Test]
        public void OperatorEquals_Diff_False()
        {
            var first = new ChromosomeStub(1.0);
            var second = new ChromosomeStub(2.0);

            Assert.IsFalse(first == second);
        }

        [Test]
        public void OperatorEquals_Equals_True()
        {
            var first = new ChromosomeStub(1.0);
            var second = new ChromosomeStub(1.0);

            Assert.IsTrue(first == second);
        }

        [Test]
        public void OperatorDiff_Equals_False()
        {
            var first = new ChromosomeStub(1.0);
            var second = new ChromosomeStub(1.0);

            Assert.IsFalse(first != second);
        }

        [Test]
        public void OperatorDiff_Diff_True()
        {
            var first = new ChromosomeStub(1.0);
            var second = new ChromosomeStub(2.0);

            Assert.IsTrue(first != second);
        }

        [Test]
        public void OperatorLowerThan_ReferenceEquals_False()
        {
            var first = new ChromosomeStub(1.0);
            var second = first;

            Assert.IsFalse(first < second);
        }

        [Test]
        public void OperatorLowerThan_FirstNull_True()
        {
            var second = new ChromosomeStub(2.0);

            Assert.IsTrue(null < second);
        }

        [Test]
        public void OperatorLowerThan_SecondNull_False()
        {
            var first = new ChromosomeStub(2.0);

            Assert.IsFalse(first < null);
        }

        [Test]
        public void OperatorLowerThan_FitnessGreater_False()
        {
            var first = new ChromosomeStub(3.0);
            var second = new ChromosomeStub(2.0);

            Assert.IsFalse(first < second);
        }

        [Test]
        public void OperatorLowerThan_FitnessLower_True()
        {
            var first = new ChromosomeStub(1.0);
            var second = new ChromosomeStub(2.0);

            Assert.IsTrue(first < second);
        }

        [Test]
        public void OperatorGreaterThan_DiffAndFitnessGreater_True()
        {
            var first = new ChromosomeStub(3.0);
            var second = new ChromosomeStub(2.0);

            Assert.IsTrue(first > second);
            Assert.IsFalse(second > first);
        }

        [Test]
        public void OperatorGreaterThan_Equals_False()
        {
            var first = new ChromosomeStub(2.0);
            var second = new ChromosomeStub(2.0);

            Assert.IsFalse(first > second);
            Assert.IsFalse(second > first);
        }
    }
}