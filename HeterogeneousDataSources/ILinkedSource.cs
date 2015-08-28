﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HeterogeneousDataSources {
    public interface ILinkedSource<TModel> {
        TModel Model { get; set; }
    }
}