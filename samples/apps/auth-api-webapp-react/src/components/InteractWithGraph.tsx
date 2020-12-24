// Copyright (c) Microsoft. All rights reserved.

import { Body1, Button, Image, Textarea, Title3 } from '@fluentui/react-components';
import React, { FC } from 'react';
import wordLogo from '../../src/word.png';
import { useSemanticKernel } from '../hooks/useSemanticKernel';
import { IKeyConfig } from '../model/KeyConfig';
import InteractionButton from './InteractionButton';

interface IData {
    uri: string;
    config: IKeyConfig;
    onBack: () => void;
}

const InteractWithGraph: FC<IData> = ({ uri, config, onBack }) => {
    const sk = useSemanticKernel(uri);
    const defaultText = `A glacier is a persistent body of dense ice that is constantly moving under its own weight. A glacier forms where the accumulati