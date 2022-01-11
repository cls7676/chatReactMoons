// Copyright (c) Microsoft. All rights reserved.

import {
    Body1,
    Button,
    Menu,
    MenuItem,
    MenuList,
    MenuPopover,
    MenuTrigger,
    Spinner,
    Subtitle1,
    Title3
} from '@fluentui/react-components';
import {
    Book24Regular,
    CheckmarkCircle24Regular,
    Code24Regular,
    PlayCircle24Regular,
    Thinking24Regular
} from '@fluentui/react-icons';
import { FC, useEffect, useState } from 'react';
import { useSemanticKernel } from '../hooks/useSemanticKernel';
import { IAsk, IAskInput } from '../model/Ask';
import { IKeyConfig } from '../model/KeyConfig';
import { IBookResult, IPage } from '../model/Page';

inter