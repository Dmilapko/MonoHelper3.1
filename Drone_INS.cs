using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MathNet.Numerics;
using MathNet.Numerics.Distributions;
using MathNet.Numerics.Statistics;
using Microsoft.Xna.Framework;

namespace MonoHelper
{
    public class DroneINS
    {
        public PointD res_pos = new PointD(0, 0);
        PointD sum_v = new PointD(0, 0);
        PointD prev_dif = new PointD(0, 0);
        public int res_t = 0;
        public double res_rot = 0;
        public double sum_rot = 0;
        double prev_rot = 0;
        public Dictionary<int, List<double>> probabilities = new Dictionary<int, List<double>>();
        int prec = 100;

        public DroneINS(int approx_t, int prec, int res_c = 6)
        {
            probabilities.Add(approx_t - 1, ProbabilityBell(approx_t - 1, res_c, approx_t * prec));
            probabilities.Add(approx_t + 1, ProbabilityBell(approx_t + 1, res_c, approx_t * prec));
            probabilities.Add(approx_t, ProbabilityBell(approx_t, res_c, approx_t * prec));
            this.prec = prec;
        }

        

        public void CalcSpeed(PointD pos_dif, double rotation_dif)
        {
            rotation_dif = (int)Math.Round(rotation_dif * 7505.74711621 * 60) / 7505.74711621 / 60;
            pos_dif = new PointD((int)Math.Round(pos_dif.X * 60 / 9.8 * 8192) * 9.8 / 60d / 8192d, (int)Math.Round(pos_dif.Y * 60 / 9.8 * 8192) * 9.8 / 60d /8192d);

            sum_rot += rotation_dif;
            res_rot += sum_rot;
            sum_v += pos_dif.Turn(prev_rot+res_rot);
            res_pos += sum_v;
            res_t++;
            prev_dif = pos_dif;
            res_rot %= 2 * Math.PI;
        }

        List<double> ProbabilityBell(int lres_t, int res_c, int prec)
        {
            List<double> pos_res = new List<double>(1<<res_c - 1);
            List<double> sums = new List<double>(prec);
            double i_coef = lres_t / (double)prec;
            long n_fact_int = 1;
            for (int i = 2; i <= lres_t; i++)
            {
                n_fact_int *= i;
            }
            double k_fact = (double)n_fact_int;
            double prev = 1;
            double sum = 0;
            for (int i = 0; i < prec; i++)
            {
                double x = i * i_coef;
                double now_g = (k_fact / SpecialFunctions.Gamma(x + lres_t + 1)) * (k_fact / SpecialFunctions.Gamma(-x + lres_t + 1));
                sums.Add(sum + (prev + now_g) / 2d);
                prev = now_g;
                sum = sums.Last();
            }
            double diap = sum;
            for (int i = 0; i < res_c; i++)
            {
                double search_s = diap / 2;
                for (int j = 0; j < 1 << i; j++) 
                {
                    int now_track = 0;
                    int l = 0, r = sums.Count - 1;
                    while (l < r)
                    {
                        now_track = (l + r + 1) / 2;
                        if (sums[now_track] > search_s) r = now_track - 1;
                        else l = now_track;
                    }
                    pos_res.Add(now_track * i_coef);
                    //pos_res.Add(search_s);
                    search_s += diap;
                }
                diap/= 2;
            }
            return pos_res;
        }

        public List<double> GetMyProbability()
        {
            if (probabilities.ContainsKey(res_t)) return probabilities[res_t];
            else
            {
                var l = ProbabilityBell(res_t, 6, res_t * prec);
                probabilities.Add(res_t, l);
                return l;
            }
        }

/*        public void CalcSpeed(double n_s, double n_a, double t = 1 / 60d)
        {
            res_pos += (new PointD(0, prev_s).Turn(prev_a) - new PointD(0, n_s).Turn(n_a)) / 2 * t;
            res_rot += t;
            res_t += t;
        }*/

        public PointD GetAproxPos(double prev_speed, double prev_a)
        {
            //res_pos.Rotate(prev_a);
            //res_pos += new PointD(0, prev_speed).Turn(prev_a);
            return res_pos;
        }

        public void Reset(double speed, double rot)
        {
            prev_rot = rot;
            res_t = 0;
            sum_v = new PointD(0, speed).Turn(rot);
            res_pos = new PointD(0, 0);
        }

        public void Reset(PointD real_res_pos, double real_rotation)
        {
            sum_v.Turn(real_res_pos.Angle() - res_pos.Angle());
            sum_v *= (real_res_pos.Length() / res_pos.Length());
            if (res_rot!=0) sum_rot *= (real_rotation / res_rot);
            res_pos = new PointD(0, 0);
            res_rot = 0;
            res_t = 0;
            prev_rot += real_rotation;
        }
    }
}
