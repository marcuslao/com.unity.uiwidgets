using System;
using System.Collections.Generic;
using Unity.UIWidgets.async;
using Unity.UIWidgets.Editor;
using Unity.UIWidgets.foundation;
using Unity.UIWidgets.widgets;
using UnityEditor;
using UnityEngine;

namespace Editor.Tests.Stream
{
    public class TestMain : UIWidgetsEditorPanel
    {
        [MenuItem("UIWidgets/Test/Stream")]
        public static void StartTest()
        {
            CreateWindow<TestMain>();
        }
        
        protected override void main()
        {
            ui_.runApp(new TestApp());
        }


        public class TestApp : StatelessWidget
        {
            /**
             * Test Stream.periodic
             */
            private void test1()
            {
                var myStream = Stream<int>.periodic(new TimeSpan(0,0,0,1), t =>
                {
                    Debug.Log("lalalala");
                    return t;
                });

                myStream.listen(val =>
                {
                    Debug.Log("value = " + val);
                });
            }
            /**
             * Test OnDone/OnData/Stream.fromIterable
             */
            private void test2()
            {
                IEnumerable<int> count()
                {
                    for (int i = 1; i < 5; i++)
                    {
                        if (i == 4)
                        {
                            throw new Exception("Intentional exception");
                        }
                        else
                        {
                            yield return i;
                        }
                    }
                }

                void sumStream(Stream<int> stream, Action<int> onDone)
                {
                    var sum = 0;
                    stream.listen(val =>
                    {
                        sum += val;
                        Debug.Log("sum stream = " + sum);
                    }, 
                        onDone: () =>
                        {
                            onDone.Invoke(sum);
                        },
                        onError: (e, stack) =>
                        {
                            Debug.Log("error at " + stack);
                        });
                }

                var myStream = Stream<int>.fromIterable(count());

                sumStream(myStream, val =>
                {
                    Debug.Log("sum = " + (int)val);
                });
            }
            
            /**
             * Test ErrorHandler
             */
            private void test3()
            {
                IEnumerable<int> count()
                {
                    for (int i = 1; i < 5; i++)
                    {
                        yield return i;
                    }
                }

                void sumStream(Stream<int> stream, Action<int> onDone)
                {
                    var sum = 0;
                    stream.listen(val =>
                        {
                            sum += val;
                            Debug.Log("sum stream = " + sum);
                        }, 
                        onDone: () =>
                        {
                            onDone.Invoke(sum);
                        },
                        onError: (e, stack) =>
                        {
                            Debug.Log("error at " + stack);
                        });
                }

                var myStream = Stream<int>.fromIterable(count());

                sumStream(myStream, val =>
                {
                    Debug.Log("sum = " + (int)val);
                });
            }

            /**
             * Test streamTransform Where
             */
            private void test4()
            {
                Stream<int> numbers = Stream<int>.fromIterable(new List<int> {0, 1, 2, 3}).where(n => n % 2 == 0);
                numbers.listen(n =>
                {
                    Debug.Log("num = " + n);
                });
            }

            private void test5()
            {
                Stream<int> numbers = Stream<int>.periodic(new TimeSpan(0, 0, 0, 1), t => t).take(3);
                numbers.listen(n =>
                {
                    Debug.Log("num = " + n);
                }, onDone: () =>
                {
                    Debug.Log("periodic finished");
                });
            }
            
            public override Widget build(BuildContext context)
            {
                test5();
                return new Container();
            }
        }
    }
}